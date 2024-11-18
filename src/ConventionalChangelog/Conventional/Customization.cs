using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace ConventionalChangelog.Conventional;

internal class Customization : ICustomization, IComparer<string>
{
    private const string TokenGroupId = "token";
    private const string BreakingGroupId = "breaking";
    private const string InnerGroupId = "inner";

    // 'a' is a randomly chosen letter, the important thing is that the
    // 'indicator' matches the breaking change pattern. The match is
    // achieved by the '!' after the 'a'
    private const string BreakingChangeIndicator = "a!";

    private readonly ImmutableArray<CommitType> _commitTypes;
    private readonly string _versionTagPrefix;
    private readonly ChangelogOrder _changelogOrder;
    private readonly string _footerPattern;
    private readonly string _semanticVersionPattern;
    private readonly bool _ignorePrerelease;
    private readonly ImmutableDictionary<string, Scope> _scopes;

    public Customization(IConfiguration configuration)
    {
        _commitTypes = [..configuration.CommitTypes];
        _versionTagPrefix = configuration.VersionTagPrefix;
        _changelogOrder = configuration.ChangelogOrder;
        _footerPattern = configuration.FooterPattern;
        _semanticVersionPattern = configuration.SemanticVersionPattern;
        _ignorePrerelease = configuration.IgnorePrerelease;
        Separator = configuration.HeaderTypeDescriptionSeparator;
        IgnoreScope = configuration.IgnoreScope;
        _scopes = configuration.Scopes.ToImmutableDictionary(x => x.Indicator, x => x);
        Relationships =
        [
            new Relationship(configuration.DropSelf, true, false),
            new Relationship(configuration.DropOther, false, true),
            new Relationship(configuration.DropBoth, true, true),
        ];

        Validate(_footerPattern);
    }


    public string Separator { get; }
    public bool IgnoreScope { get; }

    public Scope ScopeFor(string scopeIndicator)
    {
        return _scopes.TryGetValue(scopeIndicator, out var scope)
            ? scope
            : new Scope(scopeIndicator, scopeIndicator);
    }

    public IReadOnlyCollection<Relationship> Relationships { get; }

    public string Sanitize(string typeIndicator, IEnumerable<CommitMessage.Footer> footers)
    {
        if (footers.OfType<IPrintReady>().SingleOrDefault() is {} p)
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
            ? new PrintReadyFooter(token, value, BreakingChangeIndicator)
            : new CommitMessage.Footer(token, value);
    }

    private record PrintReadyFooter(string Token, string Value, string TypeIndicator)
        : CommitMessage.Footer(Token, Value), IPrintReady
    {
        public string Scope => "";
        public string Description => Value;
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
}
