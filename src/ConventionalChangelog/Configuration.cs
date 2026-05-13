namespace ConventionalChangelog;

public class Configuration(
    bool? showHash = null,
    ChangelogOrder? changelogOrder = null,
    bool? ignorePrerelease = null,
    string? versionTagPrefix = null,
    bool? skipTitle = null,
    IReadOnlyCollection<CommitType>? commitTypes = null,
    IReadOnlyCollection<Scope>? scopes = null,
    bool? ignoreScope = null,
    string? referenceCommit = null,
    IReadOnlyCollection<string>? includeDirectories = null)
    : IConfiguration
{
    public bool ShowHash => showHash ?? false;
    public IReadOnlyCollection<string> IncludeDirectories => includeDirectories ?? [];
    private static readonly DefaultConfiguration Default = new();
    public string MessageOverridePath => Default.MessageOverridePath;

    public string FooterPattern => Default.FooterPattern;

    public string VersionTagPrefix => versionTagPrefix ?? Default.VersionTagPrefix;

    public string SemanticVersionPattern => Default.SemanticVersionPattern;
    public string? ReferenceCommit => referenceCommit;

    public bool IgnorePrerelease => ignorePrerelease ?? false;

    public IEnumerable<CommitType> CommitTypes { get; } = (commitTypes ?? []).Concat(Default.CommitTypes);
    public IEnumerable<Scope> Scopes => scopes ?? Default.Scopes;

    public ChangelogOrder ChangelogOrder => changelogOrder ?? Default.ChangelogOrder;

    public string DropSelf => Default.DropSelf;
    public string DropBoth => Default.DropBoth;
    public string DropOther => Default.DropOther;
    public string HeaderTypeDescriptionSeparator => Default.HeaderTypeDescriptionSeparator;
    public bool IgnoreScope => ignoreScope ?? Default.IgnoreScope;
    public bool SkipTitle => skipTitle ?? Default.SkipTitle;

    private class DefaultConfiguration : IConfiguration
    {
        private static class Constants
        {
            // language=regex
            private const string BreakingChangeTokenPattern = "(?<breaking>(?<token>BREAKING[ -]CHANGE))(: | #)";
            // language=regex
            private const string TrailerTokenPattern = @"(?<token>[\w\-]+)(: | #)";
            // language=regex
            private const string YouTrackTokenPattern = @"#(?<token>\w+-\d+)";
            // language=regex
            public const string FooterPattern = $"^{BreakingChangeTokenPattern}|{TrailerTokenPattern}|{YouTrackTokenPattern}";
            // language=regex
            public const string VersionTagPrefix = "v";
            // language=regex
            public const string SemanticVersionPattern = @"(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";
            // language=regex
            public const string DropSelf = "fix(es|up)|enhances";
            // language=regex
            public const string DropBoth = "reverts?";
            // language=regex
            public const string DropOther = "overrides?";

            public const ChangelogOrder ChangelogOrder = default;

            // see https://www.conventionalcommits.org/en/v1.0.0/#specification
            public const string HeaderTypeDescriptionSeparator = ": ";

            public static readonly CommitType[] CommitTypes =
            [
                new("(?<inner>[a-z]+)!", "Breaking Changes", Relevance.Show),
                new("feat", "Features", Relevance.Show),
                new("fix", "Bug Fixes", Relevance.Show),
                new("perf", "Performance Improvements", Relevance.Show),
            ];

            public static readonly Scope[] Scopes = [];
            public static readonly string[] IncludeDirectories = [];
        }

        public string FooterPattern => Constants.FooterPattern;
        public string VersionTagPrefix => Constants.VersionTagPrefix;
        public string SemanticVersionPattern => Constants.SemanticVersionPattern;
        public IEnumerable<CommitType> CommitTypes => Constants.CommitTypes;
        public ChangelogOrder ChangelogOrder => Constants.ChangelogOrder;
        public string DropSelf => Constants.DropSelf;
        public string DropBoth => Constants.DropBoth;
        public string DropOther => Constants.DropOther;
        public string HeaderTypeDescriptionSeparator => Constants.HeaderTypeDescriptionSeparator;
        public bool IgnorePrerelease => false;
        public bool IgnoreScope => false;
        public IEnumerable<Scope> Scopes => Constants.Scopes;
        public bool SkipTitle => false;
        public string? ReferenceCommit => null;
        public bool ShowHash => false;
        public IReadOnlyCollection<string> IncludeDirectories => Constants.IncludeDirectories;
        public string MessageOverridePath => ".conventional-changelog";
    }
}
