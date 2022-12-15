namespace ConventionalReleaseNotes.Conventional;

public record CommitType(
    string Indicator,
    string ChangelogGroupHeader,
    Relevance Relevance = Relevance.Show);
