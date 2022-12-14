using ConventionalReleaseNotes.Conventional;
using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalReleaseNotes.Unit.Tests.CommitTypeFor;

namespace ConventionalReleaseNotes.Unit.Tests.Changelog_specs;

public class A_changelog_from_changelog_relevant_conventional_commits
{
    public static readonly object[][] ChangelogRelevantCommitTypes =
    {
        new object[] { Feature },
        new object[] { Bugfix },
        new object[] { PerformanceImprovement },
    };

    private readonly Model.Changelog _changelog = Model.Changelog.Empty;

    [Theory]
    [MemberData(nameof(ChangelogRelevantCommitTypes))]
    public void is_the_changelog_header_plus_a_group_containing_the_descriptions(CommitType type)
    {
        var message1 = type.CommitWithDescription(1);
        var message2 = type.CommitWithDescription(2);

        var changelog = Changelog.From(message1, message2);

        changelog.Should().Be(_changelog.WithGroup(type, 1, 2));
    }

    [Fact]
    public void and_irrelevant_commits_contains_all_relevant_entries()
    {
        var message1 = Feature.CommitWithDescription(1);
        var message2 = Irrelevant.CommitWith(Model.Description(2));

        var changelog = Changelog.From(message1, message2);

        changelog.Should().Be(_changelog.WithGroup(Feature, 1));
    }

    [Fact]
    public void in_random_order_is_for_each_type_the_changelog_header_plus_a_group_containing_the_descriptions()
    {
        var messages = new[]
        {
            Feature.CommitWithDescription(1),
            Bugfix.CommitWithDescription(2),
            PerformanceImprovement.CommitWithDescription(3),
            Feature.CommitWithDescription(4),
            PerformanceImprovement.CommitWithDescription(5),
            Bugfix.CommitWithDescription(6),
        };

        var changelog = Changelog.From(messages);

        changelog.Should().Be(_changelog
            .WithGroup(Feature, 1, 4)
            .WithGroup(Bugfix, 2, 6)
            .WithGroup(PerformanceImprovement, 3, 5));
    }

    public class With_breaking_change
    {
        private readonly Model.Changelog _changelog =
            new A_changelog_from_changelog_relevant_conventional_commits()._changelog;

        private const string BreakingChangesHeader = "Breaking Changes";
        private static readonly CommitType BreakingChange = new("", BreakingChangesHeader);

        [Fact]
        public void commit_type_contains_message_within_special_breaking_changes_group()
        {
            var breakingChange = Breaking(Feature).CommitWithDescription(1);

            var changelog = Changelog.From(breakingChange);

            changelog.Should().Be(_changelog
                .WithGroup(BreakingChange, 1));
        }

        [Fact]
        public void commit_type_and_non_breaking_commit_type_contains_breaking_changes_as_first_group()
        {
            var breakingChange = Breaking(Feature).CommitWithDescription(1);
            var anotherChange = Feature.CommitWithDescription(2);

            var changelog = Changelog.From(breakingChange, anotherChange);

            changelog.Should().Be(_changelog
                .WithGroup(BreakingChange, 1)
                .WithGroup(Feature, 2));
        }

        public static readonly object[][] BreakingChangeFooterTokens =
        {
            new object[] {"BREAKING CHANGE" },
            new object[] {"BREAKING-CHANGE" },
        };

        [Theory]
        [MemberData(nameof(BreakingChangeFooterTokens))]
        public void footer_contains_the_breaking_change_description_followed_by_the_commit_description_for(string footerToken)
        {
            var breakingChange = Feature.CommitWithDescription(1);
            breakingChange += NewLine + NewLine + footerToken + ": " + Model.Description(2);

            var changelog = Changelog.From(breakingChange);

            changelog.Should().Be(_changelog
                .WithGroup(BreakingChange, 2)
                .WithGroup(Feature, 1));
        }

        [Theory]
        [MemberData(nameof(BreakingChangeFooterTokens))]
        public void footer_and_breaking_type_contains_the_breaking_change_description_followed_by_the_commit_description_for(string footerToken)
        {
            var breakingChange = Breaking(Feature).CommitWithDescription(1);
            breakingChange += NewLine + NewLine + footerToken + ": " + Model.Description(2);

            var changelog = Changelog.From(breakingChange);

            changelog.Should().Be(_changelog
                .WithGroup(BreakingChange, 2)
                .WithGroup(Feature, 1));
        }
    }
}
