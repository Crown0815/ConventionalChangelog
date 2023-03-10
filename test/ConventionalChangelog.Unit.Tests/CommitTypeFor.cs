using ConventionalChangelog.Conventional;

namespace ConventionalChangelog.Unit.Tests;

internal static class CommitTypeFor
{
    public static readonly CommitType Feature = new("feat", "Features");
    public static readonly CommitType Bugfix = new("fix", "Bug Fixes");
    public static readonly CommitType PerformanceImprovement = new("perf", "Performance Improvements");

    public static readonly CommitType Irrelevant = new("chore", "", Relevance.Hide);

    public static CommitType ToCommitType(this string indicator) => new(indicator, "");
}
