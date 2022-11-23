namespace ConventionalReleaseNotes.Unit.Tests;

internal static class CommitType
{
    public static readonly ConventionalCommitType Feature = new("feat", "Features");
    public static readonly ConventionalCommitType Bugfix = new("fix", "Bug Fixes");
    public static readonly ConventionalCommitType PerformanceImprovement = new("perf", "Performance Improvements");
}
