using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static System.Environment;

namespace ConventionalReleaseNotes.Unit.Tests;

public partial class Changelog_specs
{
    private const string ChangelogHeader = "# Changelog";

    private static readonly string EmptyChangeLog = ChangelogHeader + NewLine;
    private static readonly string HeaderSeparator = NewLine + NewLine;
    private static string BulletPoint(string content) => $"- {content}{NewLine}" ;

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
        changelog.Should().Be(EmptyChangeLog);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void A_changelog_from_non_conventional_commits_is_empty(string nonConventionalCommits)
    {
        var changelog = Changelog.From(nonConventionalCommits);
        changelog.Should().Be(EmptyChangeLog);
    }

    public class A_changelog_from_changelog_irrelevant_conventional_commits
    {
        private static string Description(int index) => $"Some Description{index}";
        private static string Conventional(string type, string summary) => $"{type}: {summary}";

        private static readonly string[] ChangelogIrrelevantCommitTypeIndicators =
            { "build", "chore", "ci", "docs", "style", "refactor", "test" };

        public static readonly IEnumerable<object[]> ChangelogIrrelevantCommitTypes =
            ChangelogIrrelevantCommitTypeIndicators
            .Select(x => new object[] { new ConventionalCommitType(x, "") });

        [Theory]
        [MemberData(nameof(ChangelogIrrelevantCommitTypes))]
        public void changelog_irrelevant_types_contains_general_code_improvements_message(ConventionalCommitType type)
        {
            var conventionalCommit1 = Conventional(type.Indicator, Description(1));
            var conventionalCommit2 = Conventional(type.Indicator, Description(2));
            var changelog = Changelog.From(conventionalCommit1, conventionalCommit2);
            changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                                  "*General Code Improvements*");
        }
    }
}
