namespace ConventionalReleaseNotes;

public record ConventionalCommitType(string Indicator, string Header, bool HideFromChangelog = false);
