namespace ConventionalReleaseNotes.Conventional;

public record CommitType(string Indicator, string Header, bool HideFromChangelog = false);
