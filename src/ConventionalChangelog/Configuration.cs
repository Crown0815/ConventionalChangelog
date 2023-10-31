using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Configuration
{
    // language=regex
    private const string BreakingChangeTokenPattern = "(?<breaking>(?<token>BREAKING[ -]CHANGE))(: | #)";
    // language=regex
    private const string TrailerTokenPattern = @"(?<token>[\w\-]+)(: | #)";
    // language=regex
    private const string YouTrackTokenPattern = @"#(?<token>\w+-\d+)";

    private const string CompleteFooterPattern = $"^{BreakingChangeTokenPattern}|{TrailerTokenPattern}|{YouTrackTokenPattern}";

    // language=regex
    private const string SemanticVersionPattern2 = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?";

    public string FooterPattern => CompleteFooterPattern;

    // language=regex
    public string VersionTagPrefix => "[pv]";

    public string SemanticVersionPattern => SemanticVersionPattern2;

    public CommitType[] CommitTypes { get; } =
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

    public ChangelogOrder ChangelogOrder { get; init; }
}
