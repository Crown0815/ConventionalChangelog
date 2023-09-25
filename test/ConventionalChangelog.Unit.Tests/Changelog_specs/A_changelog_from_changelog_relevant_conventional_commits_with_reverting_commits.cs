using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_reverting_commits
    {
        private const string DefaultRevertToken = "Reverts";
        private readonly Commit _reverted;
        private readonly Commit _reverting;

        public With_reverting_commits()
        {
            _reverted = CommitTypeFor.Feature.CommitWithDescription(1);
            _reverting = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(DefaultRevertToken, _reverted.Hash);
        }

        [Fact]
        public void when_the_reverted_commit_is_part_of_the_changelog_but_the_reverting_one_is_not_shows_the_reverted_commit()
        {
            var changelog = A.Changelog.From(_reverted);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_the_reverted_and_reverting_commit_are_part_of_the_changelog_shows_neither()
        {
            var changelog = A.Changelog.From(_reverting, _reverted);

            changelog.Should().Be(A.Changelog.Empty);
        }

        [Fact]
        public void when_the_reverting_commit_is_part_of_the_changelog_but_the_reverted_one_is_not_shows_the_reverting_commit()
        {
            var changelog = A.Changelog.From(_reverting);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2));
        }

        [Fact]
        public void when_a_reverted_reverting_commit_is_part_of_the_changelog_shows_the_originally_reverted_commit()
        {
            var revertingReverting =
                CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultRevertToken, _reverting.Hash);

            var changelog = A.Changelog.From(revertingReverting, _reverting, _reverted);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_multiple_reverting_commits_target_a_single_commit_that_is_part_of_the_changelog_shows_none_of_these_commits()
        {
            var reverting2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultRevertToken, _reverted.Hash);

            var changelog = A.Changelog.From(reverting2, _reverting, _reverted);

            changelog.Should().Be(A.Changelog.Empty);
        }

        [Theory]
        [CaseVariantData(DefaultRevertToken)]
        public void recognizes_reverting_commits_by_the(string footer)
        {
            var reverting = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(footer, _reverted.Hash);
            var changelog = A.Changelog.From(reverting, _reverted);

            changelog.Should().Be(A.Changelog.Empty);
        }
    }
}
