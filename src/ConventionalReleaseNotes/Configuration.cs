using ConventionalReleaseNotes.Conventional;

namespace ConventionalReleaseNotes;

internal static class Configuration
{
    public static readonly CommitType[] CommitTypes =
    {
        new("[a-z]+!", "Breaking Changes"),
        new("feat", "Features"),
        new("fix", "Bug Fixes"),
        new("perf", "Performance Improvements"),
        new("build", "", true),
        new("chore", "", true),
        new("ci", "", true),
        new("docs", "", true),
        new("style", "", true),
        new("refactor", "", true),
        new("test", "", true),
    };
}
