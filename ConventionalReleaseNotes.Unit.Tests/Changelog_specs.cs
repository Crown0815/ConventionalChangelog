using Shouldly;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Changelog_specs
{
    private const string ChangelogHeader = "# Changelog";

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void A_changelog_with_no_changes_is_just_the_changelog_header(string noChanges)
    {
        var changelog = Changelog.From(noChanges);
        changelog.ShouldContain(ChangelogHeader);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void A_changelog_from_non_conventional_commits_is_just_the_changelog_header(string nonConventionalCommits)
    {
        var changelog = Changelog.From(nonConventionalCommits);
        changelog.ShouldContain(ChangelogHeader);
    }


}
