using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration
{
    public string VersionTagPrefix => "[pv]";
    private static readonly ImmutableArray<CommitType> DefaultCommitTypes = new[]
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
    }.ToImmutableArray();

    public Configuration()
    {
        var commitTypes = DefaultCommitTypes;
        CommitTypes = commitTypes;
        Comparer = new CommitTypeComparer(commitTypes);
    }

    public IEnumerable<CommitType> CommitTypes { get; }
    public IComparer<CommitType> Comparer { get; }

    private class CommitTypeComparer : IComparer<CommitType>
    {
        private readonly ImmutableArray<CommitType> _map;

        public CommitTypeComparer(IEnumerable<CommitType> commitTypes)
        {
            _map = commitTypes.ToImmutableArray();
        }

        public int Compare(CommitType? x, CommitType? y) =>
            IndexOf(x).CompareTo(IndexOf(y));

        private int IndexOf(CommitType? c) => c is not null
            ? _map.IndexOf(c)
            : 0;
    }
}
