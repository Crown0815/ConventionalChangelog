namespace ConventionalChangelog.Conventional;

public record CommitType(string Indicator, string ChangelogGroupHeader, Relevance Relevance)
{
    public static readonly CommitType None = new("", "", Relevance.Ignore);
}
