using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_overriding_commits
    {
        private const string OverrideToken = @"override";
        private readonly Commit _overridden;
        private readonly Commit _overriding;

        public With_overriding_commits()
        {
            _overridden = CommitTypeFor.Feature.CommitWithDescription(1);
            _overriding = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(OverrideToken, _overridden.Hash);
        }

        [Fact]
        public void when_the_overridden_commit_is_part_of_the_changelog_but_the_overriding_one_is_not_shows_the_overridden_commit()
        {
            var changelog = Changelog.From(_overridden);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_the_overridden_and_overriding_commit_are_part_of_the_changelog_shows_the_overriding_commit()
        {
            var changelog = Changelog.From(_overriding, _overridden);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2));
        }

        [Fact]
        public void when_the_overriding_commit_is_part_of_the_changelog_but_the_overridden_one_is_not_shows_the_overriding_commit()
        {
            var changelog = Changelog.From(_overriding);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2));
        }

        [Fact]
        public void when_a_overridden_overriding_commit_is_part_of_the_changelog_shows_the_overriding_overriding_commit()
        {
            var overridingOverriding = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(OverrideToken, _overriding.Hash);

            var changelog = Changelog.From(overridingOverriding, _overriding, _overridden);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 3));
        }

        [Fact]
        public void when_multiple_overriding_commits_target_a_single_commit_that_is_part_of_the_changelog_shows_all_overriding_commits()
        {
            var reverting2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(OverrideToken, _overridden.Hash);

            var changelog = Changelog.From(reverting2, _overriding, _overridden);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 3, 2));
        }
    }
}
