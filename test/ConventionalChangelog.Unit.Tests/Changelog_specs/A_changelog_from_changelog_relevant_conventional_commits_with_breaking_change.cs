using ConventionalChangelog.Conventional;
using FluentAssertions;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_breaking_change
    {
        private const string BreakingChangesHeader = "Breaking Changes";
        private static readonly CommitType BreakingChange = new("", BreakingChangesHeader);

        private const string BreakingChangeIndicator = "!";
        private static CommitType Breaking(CommitType t) => new(t.Indicator + BreakingChangeIndicator, "");

        [Fact]
        public void commit_type_contains_message_within_special_breaking_changes_group()
        {
            var breakingChange = Breaking(Feature).CommitWithDescription(1);

            var changelog = Changelog.From(breakingChange);

            changelog.Should().Be(A.Changelog
                .WithGroup(BreakingChange, 1));
        }

        [Fact]
        public void commit_type_and_non_breaking_commit_type_contains_breaking_changes_as_first_group()
        {
            var breakingChange = Breaking(Feature).CommitWithDescription(1);
            var anotherChange = Feature.CommitWithDescription(2);

            var changelog = Changelog.From(breakingChange, anotherChange);

            changelog.Should().Be(A.Changelog
                .WithGroup(BreakingChange, 1)
                .And(Feature, 2));
        }

        public static readonly object[][] BreakingChangeFooterTokens =
        {
            new object[] { "BREAKING CHANGE" },
            new object[] { "BREAKING-CHANGE" },
        };

        [Theory]
        [MemberData(nameof(BreakingChangeFooterTokens))]
        public void footer_contains_the_breaking_change_description_followed_by_the_commit_description_for_footer(string token)
        {
            var breakingChange = Feature.CommitWithDescription(1)
                .WithFooter(token, 2);

            var changelog = Changelog.From(breakingChange);

            changelog.Should().Be(A.Changelog
                .WithGroup(BreakingChange, 2)
                .And(Feature, 1));
        }

        [Theory]
        [MemberData(nameof(BreakingChangeFooterTokens))]
        public void footer_and_breaking_type_contains_the_breaking_change_description_followed_by_the_commit_description_for_footer(string token)
        {
            var breakingChange = Breaking(Feature).CommitWithDescription(1)
                .WithFooter(token, 2);

            var changelog = Changelog.From(breakingChange);

            changelog.Should().Be(A.Changelog
                .WithGroup(BreakingChange, 2)
                .And(Feature, 1));
        }
    }
}
