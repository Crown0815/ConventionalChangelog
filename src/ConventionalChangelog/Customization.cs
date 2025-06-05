using System.Collections.Immutable;
using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal class Customization : IComparer<string>
{
    private const string TokenGroupId = "token";
    private const string BreakingGroupId = "breaking";
    private const string InnerGroupId = "inner";

    // The letter 'a' is randomly chosen. The important thing is that the
    // 'indicator' matches the breaking change pattern. The match is
    // achieved by the '!' after the 'a'
    private const string BreakingChangeIndicator = "a!";

    private readonly ImmutableArray<CommitType> _commitTypes;
    private readonly string _versionTagPrefix;
    private readonly ChangelogOrder _changelogOrder;
    private readonly string _footerPattern;
    private readonly string _semanticVersionPattern;
    private readonly bool _ignorePrerelease;
    private readonly string _separator;
    private readonly bool _ignoreScope;
    private readonly ImmutableDictionary<string, Scope> _scopes;
    private readonly bool _skipTitle;
    private readonly IConfiguration _configuration;

    public Customization(IConfiguration configuration)
    {
        _commitTypes = [..configuration.CommitTypes];
        _versionTagPrefix = configuration.VersionTagPrefix;
        _changelogOrder = configuration.ChangelogOrder;
        _footerPattern = configuration.FooterPattern;
        _semanticVersionPattern = configuration.SemanticVersionPattern;
        _ignorePrerelease = configuration.IgnorePrerelease;
        _separator = configuration.HeaderTypeDescriptionSeparator;
        _ignoreScope = configuration.IgnoreScope;
        _scopes = configuration.Scopes.ToImmutableDictionary(x => x.Indicator, x => x);
        _skipTitle = configuration.SkipTitle;
        ReferenceCommit = configuration.ReferenceCommit;
        _configuration = configuration;
        Relationships =
        [
            new Relationship(configuration.DropSelf, true, false),
            new Relationship(configuration.DropOther, false, true),
            new Relationship(configuration.DropBoth, true, true),
        ];

        Validate(_footerPattern);
    }

    public string? ReferenceCommit { get; }

    public Scope ScopeFor(string scopeIndicator)
    {
        if (_ignoreScope) return Scope.None;

        return !_scopes.TryGetValue(scopeIndicator, out var scope)
            ? new Scope(scopeIndicator, scopeIndicator)
            : scope;
    }

    public IReadOnlyCollection<Relationship> Relationships { get; }
    public string Title => _skipTitle ? "" : "# Changelog" + Environment.NewLine;

    public string Sanitize(string typeIndicator, IEnumerable<CommitMessage.Footer> footers)
    {
        if (footers.OfType<BreakingChangeFooter>().SingleOrDefault() is {} p)
            return typeIndicator.ReplaceWith(InnerTypeFor(p.TypeIndicator).Indicator, InnerGroupId);
        return typeIndicator;
    }

    public ChangelogType TypeFor(string typeIndicator)
    {
        return InnerTypeFor(typeIndicator);
    }

    private CommitType InnerTypeFor(string typeIndicator)
    {
        return _commitTypes.SingleOrDefault(typeIndicator.Matches) ?? CommitType.None;
    }

    public bool IsVersionTag(string tagName)
    {
        var match = tagName.MatchWith($"^{_versionTagPrefix + _semanticVersionPattern}$");
        if (_ignorePrerelease)
            return match.Success && IsNotPrerelease(match);
        return match.Success;
    }

    private static bool IsNotPrerelease(Match match)
    {
        return match.Groups["prerelease"].Value == "";
    }

    public IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T : IPrintReady
    {
        if (_changelogOrder == ChangelogOrder.OldestToNewest)
            logEntries = logEntries.Reverse();

        return logEntries
            .OrderBy(x => x.TypeIndicator, this)
            .ThenBy(x => x.Scope);
    }

    public int Compare(string? x, string? y) => IndexOf(x).CompareTo(IndexOf(y));

    private int IndexOf(string? c) => c is not null
        ? _commitTypes.IndexOf(InnerTypeFor(c))
        : 0;

    public bool IsFooter(string line) => line.StartMatches(_footerPattern);

    public CommitMessage.Footer FooterFrom(string line)
    {
        var match = line.MatchWith(_footerPattern);
        var token = match.Groups[TokenGroupId].Value;
        var value = line.Replace(match.Value, "");
        var isBreaking = match.Groups[BreakingGroupId].Success;

        return isBreaking
            ? new BreakingChangeFooter(token, value, BreakingChangeIndicator)
            : new CommitMessage.Footer(token, value);
    }

    private record BreakingChangeFooter(string Token, string Value, string TypeIndicator)
        : CommitMessage.Footer(Token, Value), IPrintPreparable
    {
        public IPrintReady Prepare(CommitMessage context) => new Printable(TypeIndicator, "", Value, context.Hash);
        private record Printable(string TypeIndicator, string Scope, string Description, string Hash) : IPrintReady;
    }

    private static void Validate(string footerPattern)
    {
        var allGroupIds = footerPattern.MatchesWith(@"\?\<(\w+)\>");
        var found = allGroupIds.Select(x => x.Groups[1].Value).OrderBy(x => x).Distinct().ToList();

        if (found[0] != BreakingGroupId)
            throw new InvalidFooterPatternGroupIdException(BreakingGroupId, found[0]);
        if (found[1] != TokenGroupId)
            throw new InvalidFooterPatternGroupIdException(TokenGroupId, found[1]);
        if (found.Count > 2)
            throw new InvalidFooterPatternGroupIdException("", found[2]);
    }

    private class InvalidFooterPatternGroupIdException(string expected, string found)
        : Exception(MessageFrom(expected, found))
    {
        private static string MessageFrom(string expected, string found)
        {
            return $"Expected token '{expected}' but found '{found}'";
        }
    }

    public (string, string) HeaderFrom(string? firstLine) =>
        firstLine?.Split(_separator) is [var first, var second]
            ? (first.Trim(),second.Trim())
            : ("", "");

    public string DescriptionFor(IPrintReady printReady)
    {
        return _configuration.ShowHash
            ? $"{printReady.Description} ({printReady.Hash})"
            : printReady.Description;
    }
}
