using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration : IConfiguration, IComparer<CommitType>
{
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

    private const string BreakingChangeIndicator = "!";

    public static readonly CommitType BreakingChangeType = new($"[a-z]+{BreakingChangeIndicator}",
        new ChangelogType("Breaking Changes", Show));

    private static readonly CommitType[] DefaultCommitTypes =
    {
        BreakingChangeType,
        new("feat", new ChangelogType("Features", Show)),
        new("fix", new ChangelogType("Bug Fixes", Show)),
        new("perf", new ChangelogType("Performance Improvements", Show)),
        new("build", new ChangelogType("", Hide)),
        new("chore", new ChangelogType("", Hide)),
        new("ci", new ChangelogType("", Hide)),
        new("docs", new ChangelogType("", Hide)),
        new("style", new ChangelogType("", Hide)),
        new("refactor", new ChangelogType("", Hide)),
        new("test", new ChangelogType("", Hide)),
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

    public CommitType TypeFor(string typeIndicator, IReadOnlyCollection<CommitMessage.Footer> footers)
    {
        if (footers.Any(x => x.IsBreakingChange))
            typeIndicator = typeIndicator.Replace(BreakingChangeIndicator, "");
        return _commitTypes.SingleOrDefault(typeIndicator.Matches) ?? CommitType.None;
    }

    public bool IsVersionTag(string tagName) =>
        tagName.Matches(_versionTagPrefix + SemanticVersionPattern);

    public IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType
    {
        if (_order == ChangelogOrder.OldestToNewest)
            logEntries = logEntries.Reverse();
        return logEntries.OrderBy(x => x.Type, this);
    }

    public int Compare(CommitType? x, CommitType? y) =>
        IndexOf(x).CompareTo(IndexOf(y));

    private int IndexOf(CommitType? c) => c is not null
        ? _commitTypes.IndexOf(c)
        : 0;

    public bool IsFooter(string line) => line.StartMatches(FooterPattern);

    public CommitMessage.Footer FooterFrom(string line)
    {
        var match = line.MatchWith(FooterPattern);
        var token = match.Groups["token"].Value;
        var isBreaking = match.Groups["breaking"].Value is not "";
        var value = line.Replace(match.Value, "");
        return new CommitMessage.Footer(token, value, isBreaking);
    }
}
