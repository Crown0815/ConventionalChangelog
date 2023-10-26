using System.Collections.Immutable;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration
{
    public string VersionTagPrefix => "[pv]";
    private static readonly ImmutableArray<CommitType> ConfiguredCommitTypes = new[]
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

    public IReadOnlyCollection<CommitType> CommitTypes => ConfiguredCommitTypes;
    public IComparer<CommitType> Comparer { get; } = new CommitTypeComparer();

    private class CommitTypeComparer : IComparer<CommitType>
    {
        public int Compare(CommitType? x, CommitType? y) =>
            IndexOf(x).CompareTo(IndexOf(y));

        private static int IndexOf(CommitType? c) => c is not null
            ? ConfiguredCommitTypes.IndexOf(c)
            : 0;
    }
}
