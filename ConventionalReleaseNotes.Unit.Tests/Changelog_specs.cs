using System;
using FluentAssertions;
using Xunit;
using static System.Environment;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Changelog_specs
{
    private const string ChangelogHeader = "# Changelog";
    private const string BulletPoint = "- ";

    private static readonly string EmptyChangeLog = ChangelogHeader + NewLine;
    private static readonly string HeaderSeparator = NewLine + NewLine;

    [Theory]
    [InlineData(null)]
    [InlineData(new object[] { new string[] { null! } })]
    [InlineData(new object[] { new string[] { null!, null! } })]
    public void A_changelog_from_null_throws_null_exception(string[] @null)
    {
        Action fromNull = () => Changelog.From(@null);
        fromNull.Should().Throw<NullReferenceException>();
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

    public class A_changelog_from_conventional_commits_with
    {
        private static string Conventional(string type, string summary) => $"{type}: {summary}";
        private static string Level2(string header) => $"## {header}";

        public static readonly object[][] ChangelogRelevantCommitTypes =
        {
            new object[]{new ConventionalCommitType("feat", "Features")},
            new object[]{new ConventionalCommitType("fix", "Bug Fixes")},
            new object[]{new ConventionalCommitType("perf", "Performance Improvements")},
        };

        private static string Description(int index) => $"Some Description{index}";

        [Theory]
        [MemberData(nameof(ChangelogRelevantCommitTypes))]
        public void changelog_relevant_types_is_the_changelog_header_plus_a_group_containing_the_descriptions(ConventionalCommitType type)
        {
            var conventionalCommit1 = Conventional(type.Indicator, Description(1));
            var conventionalCommit2 = Conventional(type.Indicator, Description(2));
            var changelog = Changelog.From(conventionalCommit1, conventionalCommit2);
            changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                                  Level2(type.Header) + HeaderSeparator +
                                  BulletPoint + Description(1) +
                                  BulletPoint + Description(2));
        }

        public static readonly object[][] ChangelogIrrelevantCommitTypes =
        {
            new object[]{new ConventionalCommitType("chore", "")},
        };

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
