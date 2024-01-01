using System;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class A_changelog_from
{
    public static readonly TheoryData<Commit[]> NullCases = new()
    {
        new Commit[] { null! },
        new Commit[] { null!, null! },
        new Commit[] { new (""), null!, new ("") },
    };

    [Theory]
    [MemberData(nameof(NullCases))]
    public void null_throws_exception(Commit[] nullCase)
    {
        Action fromNull = () => The.ChangelogFrom(nullCase);
        fromNull.Should().Throw<Exception>();
    }

    public static readonly TheoryData<Commit[]> EmptyCases = new()
    {
        Array.Empty<Commit>(),
        new Commit[] { new("") },
        new Commit[] { new(""), new("") },
    };

    [Theory]
    [MemberData(nameof(EmptyCases))]
    public void empty_changes_is_empty(Commit[] noChanges)
    {
        var changelog = The.ChangelogFrom(noChanges);
        changelog.Should().Be(A.Changelog.Empty);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void non_conventional_commits_is_empty(string nonConventionalCommitMessage)
    {
        var changelog = The.ChangelogFrom(new Commit(nonConventionalCommitMessage));
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
        var changelog = The.ChangelogFrom(conventionalCommit1, conventionalCommit2);
        changelog.Should().Be(A.Changelog.WithGeneralCodeImprovementsMessage());
    }
}
