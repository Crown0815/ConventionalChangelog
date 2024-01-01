using System.Text.RegularExpressions;

namespace ConventionalChangelog.Configuration;

internal class DefaultConfiguration : IConfiguration
{
    private const string TokenGroupId = "token";
    private const string BreakingGroupId = "breaking";

    private static class Default
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
        public const string VersionTagPrefix = "[pv]";
        // language=regex
        public const string SemanticVersionPattern = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?";
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
        {
            new("(?<inner>[a-z]+)!", "Breaking Changes", Relevance.Show),
            new("feat", "Features", Relevance.Show),
            new("fix", "Bug Fixes", Relevance.Show),
            new("perf", "Performance Improvements", Relevance.Show),
            new("build", "", Relevance.Hide),
            new("chore", "", Relevance.Hide),
            new("ci", "", Relevance.Hide),
            new("docs", "", Relevance.Hide),
            new("style", "", Relevance.Hide),
            new("refactor", "", Relevance.Hide),
            new("test", "", Relevance.Hide),
        };

        static Default()
        {
            var allGroupIds = Regex.Matches(FooterPattern, @"\?\<(\w+)\>");
            var found = allGroupIds.Select(x => x.Groups[1].Value).OrderBy(x => x).Distinct().ToList();

            if (found[0] != BreakingGroupId)
                throw new InvalidFooterPatternGroupIdException(BreakingGroupId, found[0]);
            if (found[1] != TokenGroupId)
                throw new InvalidFooterPatternGroupIdException(TokenGroupId, found[1]);
            if (found.Count > 2)
                throw new InvalidFooterPatternGroupIdException("", found[2]);
        }
    }

    public string FooterPattern => Default.FooterPattern;
    public string VersionTagPrefix => Default.VersionTagPrefix;
    public string SemanticVersionPattern => Default.SemanticVersionPattern;
    public IEnumerable<CommitType> CommitTypes => Default.CommitTypes;
    public ChangelogOrder ChangelogOrder => Default.ChangelogOrder;
    public string DropSelf => Default.DropSelf;
    public string DropBoth => Default.DropBoth;
    public string DropOther => Default.DropOther;
    public string HeaderTypeDescriptionSeparator => Default.HeaderTypeDescriptionSeparator;
}

public class InvalidFooterPatternGroupIdException : Exception
{
    public InvalidFooterPatternGroupIdException(string expected, string found) : base(MessageWith(expected, found))
    {
    }

    private static string MessageWith(string expected, string found)
    {
        return $"Expected token '{expected}' but found '{found}'";
    }
}
