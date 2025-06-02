using AwesomeAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_fixing_commits
    {
        private const string DefaultFixToken = "Fixes";
        private const string LegacyFixToken = "FixUp";
        private const string AlternativeFixToken = "Enhances";
        private readonly Commit _target;
        private readonly Commit _fixing;

        public With_fixing_commits()
        {
            _target = CommitTypeFor.Feature.CommitWithDescription(1);
            _fixing = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(DefaultFixToken, _target.Hash);
        }

        [Fact]
        public void when_the_fixed_commit_is_part_of_the_changelog_excludes_the_fixing_commit_from_the_changelog()
        {
            var changelog = The.ChangelogFrom(_fixing, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Theory]
        [CaseVariantData(DefaultFixToken)]
        [CaseVariantData(LegacyFixToken)]
        [CaseVariantData(AlternativeFixToken)]
        public void recognizes_fixing_commits_by_the(string footer)
        {
            var fixing = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(footer, _target.Hash);
            var changelog = The.ChangelogFrom(fixing, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void
            when_a_fixed_fixing_commit_is_part_of_the_changelog_excludes_the_fixing_commit_from_the_changelog()
        {
            var fixing2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultFixToken, _fixing.Hash);

            var changelog = The.ChangelogFrom(fixing2, _fixing, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void
            when_multiple_fixing_commits_target_a_single_commit_that_is_part_of_the_changelog_excludes_all_the_fixing_commits_from_the_changelog()
        {
            var fixing2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultFixToken, _target.Hash);

            var changelog = The.ChangelogFrom(fixing2, _fixing, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_the_fixed_commit_is_not_part_of_the_changelog_includes_fixing_commit_in_the_changelog()
        {
            var fixing2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultFixToken, "randomHash");

            var changelog = The.ChangelogFrom(fixing2, _fixing, _target);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 3, 1));
        }
    }
}
