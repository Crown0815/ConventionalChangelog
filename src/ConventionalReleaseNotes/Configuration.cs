using ConventionalReleaseNotes.Conventional;
using static ConventionalReleaseNotes.Conventional.Relevance;

namespace ConventionalReleaseNotes;

internal static class Configuration
{
    private static readonly List<CommitType> Groups = new();

    static Configuration()
    {
        Configure("[a-z]+!", "Breaking Changes", Show);
        Configure("feat", "Features", Show);
        Configure("fix", "Bug Fixes", Show);
        Configure("perf", "Performance Improvements", Show);
        Configure("build", "", Hide);
        Configure("chore", "", Hide);
        Configure("ci", "", Hide);
        Configure("docs", "", Hide);
        Configure("style", "", Hide);
        Configure("refactor", "", Hide);
        Configure("test", "", Hide);
    }

    private static void Configure(string indicator, string header, Relevance relevance)
    {
        Groups.Add(new CommitType(indicator, header, relevance));
    }

    public static CommitType BreakingChange => Groups[0];
    public static IReadOnlyCollection<CommitType> CommitTypes => Groups;
    public static IComparer<CommitType> Comparer { get; } = new CommitTypeComparer();

    private class CommitTypeComparer : IComparer<CommitType>
    {
        public int Compare(CommitType? x, CommitType? y) =>
            IndexOf(x).CompareTo(IndexOf(y));

        private static int IndexOf(CommitType? c) => c is not null
            ? Groups.IndexOf(c)
            : 0;
    }
}
