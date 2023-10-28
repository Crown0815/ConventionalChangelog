using ConventionalChangelog.Conventional;

namespace ConventionalChangelog.Unit.Tests;

internal static class CommitTypeFor
{
    public static readonly CommitType Feature = new("feat", new ChangelogType("Features", Relevance.Show));
    public static readonly CommitType Bugfix = new("fix", new ChangelogType("Bug Fixes", Relevance.Show));
    public static readonly CommitType PerformanceImprovement = new("perf", new ChangelogType("Performance Improvements", Relevance.Show));

    public static readonly CommitType Irrelevant = new("chore", new ChangelogType("", Relevance.Hide));

    public static CommitType ToCommitType(this string indicator) => new(indicator, new ChangelogType("", Relevance.Show));
}
