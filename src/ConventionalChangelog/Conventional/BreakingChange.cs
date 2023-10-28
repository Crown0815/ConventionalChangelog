namespace ConventionalChangelog.Conventional;

internal static class BreakingChange
{
    public const string Indicator = "!";

    public static readonly CommitType Type = new($"[a-z]+{Indicator}", "Breaking Changes", Relevance.Show);
}
