namespace ConventionalReleaseNotes.Conventional;

public record CommitType(
    string Indicator,
    string ChangelogGroupHeader,
    bool HideFromChangelog = false);
