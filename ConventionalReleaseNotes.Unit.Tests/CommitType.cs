namespace ConventionalReleaseNotes.Unit.Tests;

internal static class CommitType
{
    public static readonly ConventionalCommitType Feature = new("feat", "Features");
    public static readonly ConventionalCommitType Bugfix = new("fix", "Bug Fixes");
    public static readonly ConventionalCommitType PerformanceImprovement = new("perf", "Performance Improvements");

    public static ConventionalCommitType ToCommitType(this string indicator) => new(indicator, "");

    private const string BreakingChangeIndicator = "!";
    public static ConventionalCommitType Breaking(ConventionalCommitType t) => new(t.Indicator + BreakingChangeIndicator, "");
}
