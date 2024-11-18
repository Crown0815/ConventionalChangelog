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
        public void by_default_groups_messages_by_type_and_alphabetical_scope()
        {
            const string scope2 = "scope2";
            const string scope1 = "scope1";
            var changelog = The.ChangelogFrom(
                Bugfix.CommitWithDescription(0).WithScope(scope2),
                Feature.CommitWithDescription(1).WithScope(scope2),
                Feature.CommitWithDescription(2).WithScope(scope1),
                Feature.CommitWithDescription(3).WithScope(scope2),
                Feature.CommitWithDescription(4).WithScope(scope1),
                Feature.CommitWithDescription(5).WithScope(scope2),
                Bugfix.CommitWithDescription(6).WithScope(scope1),
                Bugfix.CommitWithDescription(7).WithScope(scope1),
                Bugfix.CommitWithDescription(8).WithScope(scope2)
            );

            changelog.Should().Be(A.Changelog
                .WithGroup(Feature)
                .WithScope(scope1)
                .WithBulletPoint(2).WithBulletPoint(4)
                .WithScope2(scope2)
                .WithBulletPoint(1).WithBulletPoint(3).WithBulletPoint(5)
                .WithGroup(Bugfix)
                .WithScope(scope1)
                .WithBulletPoint(6).WithBulletPoint(7)
                .WithScope2(scope2)
                .WithBulletPoint(0).WithBulletPoint(8));
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

        [Fact]
        public void when_non_empty_scopes_are_mapped_uses_the_mapped_names_as_subsection_headers()
        {
            const string scopeHeader = "My Scope";
            const string scope = "scope";
            var message = Feature.CommitWithDescription(1).WithScope(scope);

            var configuration = new Configuration(scopes: [new Scope(scope, scopeHeader)]);
            var changelog = The.ChangelogWith(configuration).From([message]);

            changelog.Should().Be(A.Changelog
                .WithGroup(Feature)
                .WithScope(scopeHeader)
                .WithBulletPoint(1));
        }

        [Fact]
        public void when_non_empty_scopes_are_mapped_leaves_empty_scopes_without_subsection()
        {
            const string scopeHeader = "My Scope";
            const string scope = "scope";
            var messageWithoutScope = Feature.CommitWithDescription(2);
            var messageWithScope = Feature.CommitWithDescription(1).WithScope(scope);

            var configuration = new Configuration(scopes: [new Scope(scope, scopeHeader)]);
            var changelog = The.ChangelogWith(configuration).From([
                messageWithScope,
                messageWithoutScope]);

            changelog.Should().Be(A.Changelog
                .WithGroup(Feature, 2)
                .WithScope2(scopeHeader)
                .WithBulletPoint(1));
        }

        [Fact]
        public void when_empty_scope_is_mapped_uses_mapped_names_as_subsection_headers()
        {
            const string scopeHeader = "My Scope";
            const string emptyScope = "";
            var message = Feature.CommitWithDescription(1);

            var configuration = new Configuration(scopes: [new Scope(emptyScope, scopeHeader)]);
            var changelog = The.ChangelogWith(configuration).From([message]);

            changelog.Should().Be(A.Changelog
                .WithGroup(Feature)
                .WithScope(scopeHeader)
                .WithBulletPoint(1));
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
