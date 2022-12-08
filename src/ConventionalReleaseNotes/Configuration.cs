using ConventionalReleaseNotes.Conventional;
using static ConventionalReleaseNotes.Conventional.Relevance;

namespace ConventionalReleaseNotes;

internal static class Configuration
{
    public static readonly CommitType[] CommitTypes =
    {
        new OrderedCommitType(1, "[a-z]+!", "Breaking Changes"),
        new OrderedCommitType(2, "feat", "Features"),
        new OrderedCommitType(3, "fix", "Bug Fixes"),
        new OrderedCommitType(4, "perf", "Performance Improvements"),
        new OrderedCommitType(5, "build", "", Hide),
        new OrderedCommitType(6, "chore", "", Hide),
        new OrderedCommitType(7, "ci", "", Hide),
        new OrderedCommitType(8, "docs", "", Hide),
        new OrderedCommitType(9, "style", "", Hide),
        new OrderedCommitType(10, "refactor", "", Hide),
        new OrderedCommitType(11, "test", "", Hide),
    };

    private record OrderedCommitType(int Index, string Indicator, string ChangelogGroupHeader, Relevance Relevance = Show)
        : CommitType(Indicator, ChangelogGroupHeader, Relevance), IComparable
    {
        public int CompareTo(object? other) => other is OrderedCommitType o
            ? Index.CompareTo(o.Index)
            : 1;
    }
}
