namespace ConventionalChangelog.Conventional;

public record CommitType(string Indicator, ChangelogType Changelog)
{
    public static readonly CommitType None = new("", new ChangelogType("", Relevance.Ignore));

}

public record ChangelogType(string GroupHeader, Relevance Relevance);
