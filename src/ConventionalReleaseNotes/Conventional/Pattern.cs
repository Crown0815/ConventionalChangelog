namespace ConventionalReleaseNotes.Conventional;

internal static class Pattern
{
    // language=regex
    public const string FooterToken = @"[\w\-]+";
    // language=regex
    public const string YouTrackCommand = @"^#\w+-\d+";
}
