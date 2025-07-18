namespace ConventionalChangelog.Unit.Tests;

internal static class CommitTypeFor
{
    private const string BreakingChangesHeader = "Breaking Changes";
    internal static readonly CommitType BreakingChange = new("", BreakingChangesHeader, Relevance.Show);
    public static readonly CommitType Feature = new("feat", "Features", Relevance.Show);
    public static readonly CommitType Bugfix = new("fix", "Bug Fixes", Relevance.Show);
    public static readonly CommitType PerformanceImprovement = new("perf", "Performance Improvements", Relevance.Show);

    public static readonly CommitType Irrelevant = new("chore", "", Relevance.Hide);

    public static CommitType ToCommitType(this string indicator) => new(indicator, "", Relevance.Show);
}
