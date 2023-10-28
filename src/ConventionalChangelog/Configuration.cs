using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration : IConfiguration, IComparer<CommitType>
{
    // language=regex
    private const string FooterTokenPattern = @"[\w\-]+";
    // language=regex
    private const string YouTrackCommandPattern = @"^#\w+-\d+";
    // language=regex
    private const string SemanticVersionPattern = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?";

    private const string FooterPattern = $"^(?<token>{FooterTokenPattern}|{BreakingChange.FooterPattern})(: | #)";
    private const string DefaultVersionTagPrefix = "[pv]";

    private static readonly CommitType[] DefaultCommitTypes =
    {
        BreakingChange.Type,
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

    public CommitType TypeFor(string typeIndicator) =>
        _commitTypes.SingleOrDefault(typeIndicator.Matches) ?? CommitType.None;

    public bool IsVersionTag(string tagName) =>
        tagName.Matches(_versionTagPrefix + SemanticVersionPattern);

    public bool IsBreakingChange(CommitMessage.Footer footer) =>
        footer.Token.Matches(BreakingChange.FooterPattern);

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

    public bool IsFooter(string line) =>
        line.StartMatches(YouTrackCommandPattern)
        || line.StartMatches(FooterPattern);

    public CommitMessage.Footer FooterFrom(string line)
    {
        if (line.StartMatches(YouTrackCommandPattern))
            return YouTrackFooterFrom(line);
        return TrailerFooterFrom(line);
    }

    private static CommitMessage.Footer TrailerFooterFrom(string line)
    {
        var match = line.MatchWith(FooterPattern);
        var token = match.Groups["token"].Value;
        var value = line.Replace(match.Value, "");
        return new CommitMessage.Footer(token, value);
    }

    private static CommitMessage.Footer YouTrackFooterFrom(string line)
    {
        var parts = line.Split(" ");
        var token = parts.First().Replace("#", "");
        var value = line.Replace("#" + token, "").Trim();
        return new CommitMessage.Footer(token, value);
    }
}
