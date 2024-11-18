namespace ConventionalChangelog;

public class Configuration(IReadOnlyCollection<Scope>? scopes = null, bool? ignoreScope = null) : IConfiguration
{
    private readonly IConfiguration _default = new DefaultConfiguration();
    private readonly ChangelogOrder? _changelogOrder;
    private readonly string? _versionTagPrefix;

    public string FooterPattern => _default.FooterPattern;

    public string VersionTagPrefix
    {
        get => _versionTagPrefix ?? _default.VersionTagPrefix;
        init => _versionTagPrefix = value;
    }

    public string SemanticVersionPattern => _default.SemanticVersionPattern;
    public bool IgnorePrerelease { get; init; }

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
}

public record Scope(string Indicator, string GroupHeader)
{
    public static Scope None { get; } = new("", null);
}
