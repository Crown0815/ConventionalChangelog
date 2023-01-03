namespace ConventionalChangelog.Conventional;

internal static class BreakingChange
{
    // language=regex
    public const string FooterPattern = "BREAKING[ -]CHANGE";

    public const string Indicator = "!";

    public static readonly CommitType Type = new($"[a-z]+{Indicator}", "Breaking Changes");
}
