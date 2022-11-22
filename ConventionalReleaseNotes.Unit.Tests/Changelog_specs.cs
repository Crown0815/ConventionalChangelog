using System;
using FluentAssertions;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests;

public partial class Changelog_specs
{
    private readonly Model.Changelog _emptyChangeLog = new Model.Changelog().WithTitle();

    [Theory]
    [InlineData(null)]
    [InlineData(new object[] { new string[] { null! } })]
    [InlineData(new object[] { new string[] { null!, null! } })]
    [InlineData(new object[] { new[] { "", null!, "" } })]
    public void A_changelog_from_null_throws_null_exception(string[] @null)
    {
        Action fromNull = () => Changelog.From(@null);
        fromNull.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData]
    [InlineData("")]
    [InlineData("", "")]
    public void A_changelog_from_empty_changes_is_empty(params string[] noChanges)
    {
        var changelog = Changelog.From(noChanges);
        changelog.Should().Be(_emptyChangeLog);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void A_changelog_from_non_conventional_commits_is_empty(string nonConventionalCommits)
    {
        var changelog = Changelog.From(nonConventionalCommits);
        changelog.Should().Be(_emptyChangeLog);
    }

    [Theory]
    [InlineData("build")]
    [InlineData("chore")]
    [InlineData("ci")]
    [InlineData("docs")]
    [InlineData("style")]
    [InlineData("refactor")]
    [InlineData("test")]
    public void A_changelog_from_changelog_irrelevant_conventional_commits_contains_general_code_improvements_message(string indicator)
    {
        var conventionalCommit1 = Model.ConventionalCommitMessage(indicator, "unused");
        var conventionalCommit2 = Model.ConventionalCommitMessage(indicator, "unused");
        var changelog = Changelog.From(conventionalCommit1, conventionalCommit2);
        changelog.Should().Be(_emptyChangeLog.WithGeneralCodeImprovementsMessage());
    }
}
