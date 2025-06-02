using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public class With_overriding_commits
    {
        private const string DefaultOverrideToken = "Overrides";
        private readonly Commit _overridden;
        private readonly Commit _overriding;

        public With_overriding_commits()
        {
            _overridden = CommitTypeFor.Feature.CommitWithDescription(1);
            _overriding = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(DefaultOverrideToken, _overridden.Hash);
        }

        [Fact]
        public void when_the_overridden_commit_is_part_of_the_changelog_but_the_overriding_one_is_not_shows_the_overridden_commit()
        {
            var changelog = The.ChangelogFrom(_overridden);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 1));
        }

        [Fact]
        public void when_the_overridden_and_overriding_commit_are_part_of_the_changelog_shows_the_overriding_commit()
        {
            var changelog = The.ChangelogFrom(_overriding, _overridden);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2));
        }

        [Fact]
        public void when_the_overriding_commit_is_part_of_the_changelog_but_the_overridden_one_is_not_shows_the_overriding_commit()
        {
            var changelog = The.ChangelogFrom(_overriding);
            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2));
        }

        [Fact]
        public void when_a_overridden_overriding_commit_is_part_of_the_changelog_shows_the_overriding_overriding_commit()
        {
            var overridingOverriding = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultOverrideToken, _overriding.Hash);

            var changelog = The.ChangelogFrom(overridingOverriding, _overriding, _overridden);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 3));
        }

        [Fact]
        public void when_multiple_overriding_commits_target_a_single_commit_that_is_part_of_the_changelog_shows_all_overriding_commits()
        {
            var overriding2 = CommitTypeFor.Feature.CommitWithDescription(3).WithFooter(DefaultOverrideToken, _overridden.Hash);

            var changelog = The.ChangelogFrom(overriding2, _overriding, _overridden);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 3, 2));
        }

        [Theory]
        [CaseVariantData(DefaultOverrideToken)]
        public void recognizes_overriding_commits_by_the(string footer)
        {
            var overridingOverriding = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(footer, _overridden.Hash);
            var changelog = The.ChangelogFrom(overridingOverriding, _overridden);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2));
        }

        [Theory, SuppressMessage("ReSharper", "StringLiteralTypo")]
        [InlineData("hoverride")]
        [InlineData("coverride")]
        [InlineData("overwrite")]
        [InlineData("overrider")]
        public void does_not_recognize_commits_as_overriding_if_they_contain_(string footer)
        {
            var overridingOverriding = CommitTypeFor.Feature.CommitWithDescription(2).WithFooter(footer, _overridden.Hash);
            var changelog = The.ChangelogFrom(overridingOverriding, _overridden);

            changelog.Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2, 1));
        }
    }
}
