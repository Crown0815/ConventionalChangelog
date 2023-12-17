using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Configuration
{
    public string FooterPattern => DefaultConfiguration.FooterPattern;

    public string VersionTagPrefix => DefaultConfiguration.VersionTagPrefix;

    public string SemanticVersionPattern => DefaultConfiguration.SemanticVersionPattern;

    public IEnumerable<CommitType> CommitTypes => DefaultConfiguration.CommitTypes;

    public ChangelogOrder ChangelogOrder { get; init; } = DefaultConfiguration.ChangelogOrder;

    public string DropSelf => DefaultConfiguration.DropSelf;
    public string DropBoth => DefaultConfiguration.DropBoth;
    public string DropOther => DefaultConfiguration.DropOther;
}
