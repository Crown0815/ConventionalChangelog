namespace ConventionalChangelog;

public interface IConfiguration
{
    string FooterPattern { get; }
    string VersionTagPrefix { get; }
    string SemanticVersionPattern { get; }
    IEnumerable<CommitType> CommitTypes { get; }
    ChangelogOrder ChangelogOrder { get; }
    string DropSelf { get; }
    string DropBoth { get; }
    string DropOther { get; }
}
