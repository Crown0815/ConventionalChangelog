using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_fix_up_commits
    {
        private const string DefaultFixUpToken = @"Fixes";
        private readonly Commit _target;
        private readonly Commit _fixUp;

        public With_fix_up_commits()
        {
            _target = CommitTypeFor.Feature.CommitWithDescription(1);
            _fixUp = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(DefaultFixUpToken, _target.Hash);
        }

        [Fact]
        public void when_the_fixed_up_commit_is_part_of_the_changelog_excludes_the_fix_up_commit_from_the_changelog()
        {
            var changelog = Changelog.From(_fixUp, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Theory]
        [InlineData(DefaultFixUpToken)]
        [InlineData(@"fixup")]
        [InlineData("FixUp")]
        [InlineData("fIXuP")]
        [InlineData(@"fIXES")]
        public void recognizes_fix_up_commits_by_the(string footer)
        {
            var fixUp = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(footer, _target.Hash);
            var changelog = Changelog.From(fixUp, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_a_fixed_up_fix_up_commit_is_part_of_the_changelog_excludes_the_fix_up_commit_from_the_changelog()
        {
            var fixUp2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultFixUpToken, _fixUp.Hash);

            var changelog = Changelog.From(fixUp2, _fixUp, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_multiple_fix_up_commits_target_a_single_commit_that_is_part_of_the_changelog_excludes_all_the_fix_up_commits_from_the_changelog()
        {
            var fixUp2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultFixUpToken, _target.Hash);

            var changelog = Changelog.From(fixUp2, _fixUp, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_the_fixed_up_commit_is_not_part_of_the_changelog_includes_fix_up_commit_in_the_changelog()
        {
            var fixUp2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultFixUpToken, "randomHash");

            var changelog = Changelog.From(fixUp2, _fixUp, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 3, 1));
        }
    }
}
