namespace ConventionalChangelog;

public class Configuration(
    bool? ignorePrerelease = null,
    string? versionTagPrefix = null,
    bool? skipTitle = null,
    IReadOnlyCollection<Scope>? scopes = null,
    bool? ignoreScope = null)
    : IConfiguration
{
    private readonly DefaultConfiguration _default = new();
    private readonly ChangelogOrder? _changelogOrder;

    public string FooterPattern => _default.FooterPattern;

    public string VersionTagPrefix => versionTagPrefix ?? _default.VersionTagPrefix;

    public string SemanticVersionPattern => _default.SemanticVersionPattern;

    public bool IgnorePrerelease => ignorePrerelease ?? false;

    public IEnumerable<CommitType> CommitTypes => _default.CommitTypes;
    public IEnumerable<Scope> Scopes => scopes ?? _default.Scopes;

    public ChangelogOrder ChangelogOrder
    {
        get => _changelogOrder ?? _default.ChangelogOrder;
        init => _changelogOrder = value;
    }

    public string DropSelf => _default.DropSelf;
    public string DropBoth => _default.DropBoth;
    public string DropOther => _default.DropOther;
    public string HeaderTypeDescriptionSeparator => _default.HeaderTypeDescriptionSeparator;
    public bool IgnoreScope => ignoreScope ?? _default.IgnoreScope;
    public bool SkipTitle => skipTitle ?? _default.SkipTitle;
}
