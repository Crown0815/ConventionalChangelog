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

            changelog.Should().Be(A.Changelog.WithGroup(Feature, 1));
        }
    }
}
