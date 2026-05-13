using System;
using System.Linq;
using AwesomeAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class A_changelog_from
{
    public static readonly TheoryData<Commit[]> NullCases =
    [
        new Commit[] { null! },
        new Commit[] { null!, null! },
        new[] { A.Commit(""), null!, A.Commit("") },
    ];

    [Theory]
    [MemberData(nameof(NullCases))]
    public void null_throws_exception(Commit[] nullCase)
    {
        Action fromNull = () => The.ChangelogFrom(nullCase);
        fromNull.Should().Throw<Exception>();
    }

    [Fact]
    public void no_changes_is_empty()
    {
        var changelog = The.ChangelogFrom();
        changelog.Should().Be(A.Changelog.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("", "")]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void non_conventional_commits_only_shows_general_code_improvements(params string[] messages)
    {
        var commits = messages.Select(A.Commit).ToArray();
        var changelog = The.ChangelogFrom(commits);
        changelog.Should().Be(A.Changelog.WithGeneralCodeImprovementsMessage());
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
        var changelog = The.ChangelogFrom(conventionalCommit1, conventionalCommit2);
        changelog.Should().Be(A.Changelog.WithGeneralCodeImprovementsMessage());
    }
}
