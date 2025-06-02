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
    string HeaderTypeDescriptionSeparator { get; }
    bool IgnorePrerelease { get; }
    bool IgnoreScope { get; }
    IEnumerable<Scope> Scopes { get; }
    bool SkipTitle { get; }
    string? ReferenceCommit { get; }
}
