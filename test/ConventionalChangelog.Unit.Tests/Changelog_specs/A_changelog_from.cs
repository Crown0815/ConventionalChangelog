using System;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class A_changelog_from
{
    [Theory]
    [InlineData(null)]
    [InlineData(new object[] { new string[] { null! } })]
    [InlineData(new object[] { new string[] { null!, null! } })]
    [InlineData(new object[] { new[] { "", null!, "" } })]
    public void null_throws_null_exception(string[] @null)
    {
        Action fromNull = () => Changelog.From(@null);
        fromNull.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData]
    [InlineData("")]
    [InlineData("", "")]
    public void empty_changes_is_empty(params string[] noChanges)
    {
        var changelog = Changelog.From(noChanges);
        changelog.Should().Be(A.Changelog.Empty);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void non_conventional_commits_is_empty(string nonConventionalCommits)
    {
        var changelog = Changelog.From(nonConventionalCommits);
        changelog.Should().Be(A.Changelog.Empty);
    }

    [Theory]
    [InlineData("build")]
    [InlineData("chore")]
    [InlineData("ci")]
    [InlineData("docs")]
    [InlineData("style")]
    [InlineData("refactor")]
    [InlineData("test")]
    public void changelog_irrelevant_conventional_commits_contains_general_code_improvements_message(string indicator)
    {
        var type = indicator.ToCommitType();
        var conventionalCommit1 = type.CommitWith("unused description");
        var conventionalCommit2 = type.CommitWith("unused description");
        var changelog = Changelog.From(conventionalCommit1, conventionalCommit2);
        changelog.Should().Be(A.Changelog.WithGeneralCodeImprovementsMessage());
    }
}