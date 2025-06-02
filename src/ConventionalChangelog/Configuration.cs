namespace ConventionalChangelog;

public class Configuration(
    ChangelogOrder? changelogOrder = null,
    bool? ignorePrerelease = null,
    string? versionTagPrefix = null,
    bool? skipTitle = null,
    IReadOnlyCollection<Scope>? scopes = null,
    bool? ignoreScope = null,
    string? referenceCommit = null)
    : IConfiguration
{
    private readonly DefaultConfiguration _default = new();

    public string FooterPattern => _default.FooterPattern;

    public string VersionTagPrefix => versionTagPrefix ?? _default.VersionTagPrefix;

    public string SemanticVersionPattern => _default.SemanticVersionPattern;
    public string? ReferenceCommit => referenceCommit;

    public bool IgnorePrerelease => ignorePrerelease ?? false;

    public IEnumerable<CommitType> CommitTypes => _default.CommitTypes;
    public IEnumerable<Scope> Scopes => scopes ?? _default.Scopes;

    public ChangelogOrder ChangelogOrder => changelogOrder ?? _default.ChangelogOrder;

    public string DropSelf => _default.DropSelf;
    public string DropBoth => _default.DropBoth;
    public string DropOther => _default.DropOther;
    public string HeaderTypeDescriptionSeparator => _default.HeaderTypeDescriptionSeparator;
    public bool IgnoreScope => ignoreScope ?? _default.IgnoreScope;
    public bool SkipTitle => skipTitle ?? _default.SkipTitle;
}
