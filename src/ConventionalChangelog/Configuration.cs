using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration : IConfiguration, IComparer<string>
{
    private const string ConventionalCommitSeparator = ": "; // see https://www.conventionalcommits.org/en/v1.0.0/#specification

    // language=regex
    private const string BreakingChangeIndicator = "(?<inner>[a-z]+)!";
    // language=regex
    private const string BreakingChangeTokenPattern = "(?<breaking>(?<token>BREAKING[ -]CHANGE))(: | #)";
    // language=regex
    private const string TrailerTokenPattern = @"(?<token>[\w\-]+)(: | #)";
    // language=regex
    private const string YouTrackTokenPattern = @"#(?<token>\w+-\d+)";

    // language=regex
    private const string DefaultVersionTagPrefix = "[pv]";
    // language=regex
    private const string SemanticVersionPattern = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?";

    private const string FooterPattern = $"^{BreakingChangeTokenPattern}|{TrailerTokenPattern}|{YouTrackTokenPattern}";

    private static readonly CommitType[] DefaultCommitTypes =
    {
        new(BreakingChangeIndicator, "Breaking Changes", Show),
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

    public static Configuration Default() => With(default);
    public static Configuration With(ChangelogOrder order) => new(DefaultCommitTypes, DefaultVersionTagPrefix, order);

    private readonly ImmutableArray<CommitType> _commitTypes;
    private readonly string _versionTagPrefix;
    private readonly ChangelogOrder _order;

    private Configuration(IEnumerable<CommitType> commitTypes, string versionTagPrefix, ChangelogOrder order)
    {
        _commitTypes = commitTypes.ToImmutableArray();
        _versionTagPrefix = versionTagPrefix;
        _order = order;
    }

    public string Separator => ConventionalCommitSeparator;

    public string Sanitize(string typeIndicator, IEnumerable<CommitMessage.Footer> footers)
    {
        if (footers.Any(x => x is IPrintable))
            return typeIndicator.ReplaceWith(BreakingChangeIndicator, "inner");
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
        tagName.Matches(_versionTagPrefix + SemanticVersionPattern);

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

    public bool IsFooter(string line) => line.StartMatches(FooterPattern);

    public CommitMessage.Footer FooterFrom(string line)
    {
        var match = line.MatchWith(FooterPattern);
        var token = match.Groups["token"].Value;
        var value = line.Replace(match.Value, "");
        var isBreaking = match.Groups["breaking"].Value is not "";
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
