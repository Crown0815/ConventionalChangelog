using System;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class A_changelog_from
{
    public static readonly object[][] NullCases =
    {
        new object[] { null! },
        new object[] { null!, null! },
        new object[] { new Commit(""), null!, new Commit("") },
    };

    [Theory]
    [MemberData(nameof(NullCases))]
    public void null_throws_exception(params Commit[] nullCase)
    {
        Action fromNull = () => A.Changelog.From(nullCase);
        fromNull.Should().Throw<Exception>();
    }

    public static readonly object[][] EmptyCases =
    {
        new object[] {  },
        new object[] { new Commit("") },
        new object[] { new Commit(""), new Commit("") },
    };

    [Theory]
    [MemberData(nameof(EmptyCases))]
    public void empty_changes_is_empty(params Commit[] noChanges)
    {
        var changelog = A.Changelog.From(noChanges);
        changelog.Should().Be(A.Changelog.Empty);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void non_conventional_commits_is_empty(string nonConventionalCommitMessage)
    {
        var changelog = A.Changelog.From(new Commit(nonConventionalCommitMessage));
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
        var changelog = A.Changelog.From(conventionalCommit1, conventionalCommit2);
        changelog.Should().Be(A.Changelog.WithGeneralCodeImprovementsMessage());
    }
}
