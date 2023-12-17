using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal static class DefaultConfiguration
{
    // language=regex
    private const string BreakingChangeTokenPattern = "(?<breaking>(?<token>BREAKING[ -]CHANGE))(: | #)";
    // language=regex
    private const string TrailerTokenPattern = @"(?<token>[\w\-]+)(: | #)";
    // language=regex
    private const string YouTrackTokenPattern = @"#(?<token>\w+-\d+)";
    // language=regex
    public const string FooterPattern = $"^{BreakingChangeTokenPattern}|{TrailerTokenPattern}|{YouTrackTokenPattern}";
    // language=regex
    public const string VersionTagPrefix = "[pv]";
    // language=regex
    public const string SemanticVersionPattern = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?";
    // language=regex
    public const string DropSelf = "fix(es|up)|enhances";
    // language=regex
    public const string DropBoth = "reverts?";
    // language=regex
    public const string DropOther = "overrides?";

    public const ChangelogOrder ChangelogOrder = default;

    public static readonly CommitType[] CommitTypes =
    {
        new("(?<inner>[a-z]+)!", "Breaking Changes", Relevance.Show),
        new("feat", "Features", Relevance.Show),
        new("fix", "Bug Fixes", Relevance.Show),
        new("perf", "Performance Improvements", Relevance.Show),
        new("build", "", Relevance.Hide),
        new("chore", "", Relevance.Hide),
        new("ci", "", Relevance.Hide),
        new("docs", "", Relevance.Hide),
        new("style", "", Relevance.Hide),
        new("refactor", "", Relevance.Hide),
        new("test", "", Relevance.Hide),
    };
}
