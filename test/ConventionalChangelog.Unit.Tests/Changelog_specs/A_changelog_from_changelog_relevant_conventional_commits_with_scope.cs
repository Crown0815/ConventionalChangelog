using FluentAssertions;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_scope
    {
        [Fact]
        public void by_default_is_the_changelog_header_plus_a_group_containing_a_group_for_each_scope()
        {
            var message = Feature.CommitWithDescription(1).WithScope("scope");

            var changelog = The.ChangelogFrom(message);

            changelog.Should().Be(A.Changelog
                .WithGroup(Feature)
                .WithScope("scope")
                .WithBulletPoint(1));
        }

        [Fact]
        public void when_the_scope_is_empty_ignores_the_scope()
        {
            var noScope = Feature.CommitWithDescription(1);
            var emptyScope = noScope.WithScope("");

            The.ChangelogFrom(noScope).Should().Be(The.ChangelogFrom(emptyScope));
        }

        [Fact]
        public void when_scope_is_configured_to_be_ignored_ignores_the_scope()
        {
            var message = Feature.CommitWithDescription(1).WithScope("scope");

            var configuration = new Configuration(ignoreScope: true);
            var changelog = The.ChangelogWith(configuration).From([message]);

            changelog.Should().Be(A.Changelog.WithGroup(Feature, 1));
        }

        [Theory]
        [InlineData(" (scope)")]
        [InlineData("(scope) ")]
        [InlineData("( scope)")]
        [InlineData("(scope )")]
        [InlineData(" ( scope ) ")]
        [InlineData("     (     scope     )    ")]
        public void produces_the_same_result_independent_of_spaces_before_after_or_within_the_scope(string withSpaces)
        {
            var reference = new Commit($"{Feature.Indicator}(scope): whatever");
            var sample = new Commit($"{Feature.Indicator}{withSpaces}: whatever");

            The.ChangelogFrom(reference).Should().Be(The.ChangelogFrom(sample));
        }
    }
}
