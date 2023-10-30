namespace ConventionalChangelog.Conventional;

public record CommitType(string Indicator, string GroupHeader, Relevance Relevance) :
    ChangelogType(GroupHeader, Relevance)
{
    public static readonly CommitType None = new("", "", Relevance.Ignore);
}
