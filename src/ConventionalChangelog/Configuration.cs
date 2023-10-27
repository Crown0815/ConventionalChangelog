using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration : ITypeFinder, IComparer<CommitType>
{
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

    public CommitType TypeFor(string typeIndicator)
    {
        return _commitTypes.SingleOrDefault(typeIndicator.Matches) ?? CommitType.None;
    }

    public bool IsVersionTag(string tagName) =>
        tagName.IsSemanticVersion(_versionTagPrefix);

    public IOrderedEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType
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
}
