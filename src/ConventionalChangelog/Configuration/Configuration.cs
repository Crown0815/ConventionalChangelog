namespace ConventionalChangelog.Configuration;

public class Configuration : IConfiguration
{
    private readonly IConfiguration _default = new DefaultConfiguration();
    private readonly ChangelogOrder? _changelogOrder;

    public string FooterPattern => _default.FooterPattern;

    public string VersionTagPrefix => _default.VersionTagPrefix;

    public string SemanticVersionPattern => _default.SemanticVersionPattern;

    public IEnumerable<CommitType> CommitTypes => _default.CommitTypes;

    public ChangelogOrder ChangelogOrder
    {
        get => _changelogOrder ?? _default.ChangelogOrder;
        init => _changelogOrder = value;
    }

    public string DropSelf => _default.DropSelf;
    public string DropBoth => _default.DropBoth;
    public string DropOther => _default.DropOther;
}
