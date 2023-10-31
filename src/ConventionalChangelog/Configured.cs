using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

internal class DefaultConfiguration
{
    // language=regex
    private const string BreakingChangeTokenPattern = "(?<breaking>(?<token>BREAKING[ -]CHANGE))(: | #)";
    // language=regex
    private const string TrailerTokenPattern = @"(?<token>[\w\-]+)(: | #)";
    // language=regex
    private const string YouTrackTokenPattern = @"#(?<token>\w+-\d+)";

    private const string CompleteFooterPattern = $"^{BreakingChangeTokenPattern}|{TrailerTokenPattern}|{YouTrackTokenPattern}";

    // language=regex
    private const string SemanticVersionPattern2 = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?";

    public string FooterPattern => CompleteFooterPattern;

    // language=regex
    public string VersionTagPrefix => "[pv]";

    public string SemanticVersionPattern => SemanticVersionPattern2;

    public CommitType[] CommitTypes { get; } =
    {
        new("(?<inner>[a-z]+)!", "Breaking Changes", Show),
        new("feat", "Features", Show),
        new("fix", "Bug Fixes", Show),
        new("perf", "Performance Improvements", Show),
        new("build", "", Hide),
        new("chore", "", Hide),
        new("ci", "", Hide),
        new("docs", "", Hide),
        new("style", "", Hide),
        new("refactor", "", Hide),
        new("test", "", Hide),
    };
}

public class Configured : IConfigured, IComparer<string>
{
    private const string ConventionalCommitSeparator = ": "; // see https://www.conventionalcommits.org/en/v1.0.0/#specification

    public static Configured Default() => With(default);
    public static Configured With(ChangelogOrder order)
    {
        var configuration = new DefaultConfiguration();
        return new Configured(configuration.CommitTypes, configuration.VersionTagPrefix, order, configuration
            .FooterPattern, configuration.SemanticVersionPattern);
    }

    private readonly ImmutableArray<CommitType> _commitTypes;
    private readonly string _versionTagPrefix;
    private readonly ChangelogOrder _order;
    private readonly string _footerPattern;
    private readonly string _semanticVersionPattern;

    private Configured(IEnumerable<CommitType> commitTypes, string versionTagPrefix, ChangelogOrder order, string footerPattern, string semanticVersionPattern)
    {
        _commitTypes = commitTypes.ToImmutableArray();
        _versionTagPrefix = versionTagPrefix;
        _order = order;
        _footerPattern = footerPattern;
        _semanticVersionPattern = semanticVersionPattern;
    }

    public string Separator => ConventionalCommitSeparator;

    public string Sanitize(string typeIndicator, IEnumerable<CommitMessage.Footer> footers)
    {
        if (footers.OfType<IPrintable>().SingleOrDefault() is {} p)
            return typeIndicator.ReplaceWith(InnerTypeFor(p.TypeIndicator).Indicator, "inner");
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

    public bool IsVersionTag(string tagName) =>
        tagName.Matches(_versionTagPrefix + _semanticVersionPattern);

    public IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T : IHasCommitType
    {
        if (_order == ChangelogOrder.OldestToNewest)
            logEntries = logEntries.Reverse();
        return logEntries.OrderBy(x => x.TypeIndicator, this);
    }

    public int Compare(string? x, string? y) => IndexOf(x).CompareTo(IndexOf(y));

    private int IndexOf(string? c) => c is not null
        ? _commitTypes.IndexOf(InnerTypeFor(c))
        : 0;

    public bool IsFooter(string line) => line.StartMatches(_footerPattern);

    public CommitMessage.Footer FooterFrom(string line)
    {
        var match = line.MatchWith(_footerPattern);
        var token = match.Groups["token"].Value;
        var value = line.Replace(match.Value, "");
        var isBreaking = match.Groups["breaking"].Success;
        if (isBreaking)
            return new PrintableFooter(token, value, "breaking!");
        return new CommitMessage.Footer(token, value);
    }

    private record PrintableFooter(string Token, string Value, string TypeIndicator)
        : CommitMessage.Footer(Token, Value), IPrintable
    {
        public string Description => Value;
    }
}
