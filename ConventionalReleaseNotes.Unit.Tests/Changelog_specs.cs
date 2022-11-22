using System;
using FluentAssertions;
using Xunit;
using static System.Environment;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Changelog_specs
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
        private static string Description(int index) => $"Some Description{index}";
        private static string Conventional(string type, string summary) => $"{type}: {summary}";
        private static string Level2(string header) => $"## {header}";

        private static readonly ConventionalCommitType Feature = new("feat", "Features");
        private static readonly ConventionalCommitType Bugfix = new("fix", "Bug Fixes");
        private static readonly ConventionalCommitType PerformanceImprovement = new("perf", "Performance Improvements");

        public static readonly object[][] ChangelogRelevantCommitTypes =
        {
            new object[]{Feature},
            new object[]{Bugfix},
            new object[]{PerformanceImprovement},
        };

        [Theory]
        [MemberData(nameof(ChangelogRelevantCommitTypes))]
        public void changelog_relevant_types_is_the_changelog_header_plus_a_group_containing_the_descriptions(ConventionalCommitType type)
        {
            var conventionalCommit1 = Conventional(type.Indicator, Description(1));
            var conventionalCommit2 = Conventional(type.Indicator, Description(2));
            var changelog = Changelog.From(conventionalCommit1, conventionalCommit2);
            changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                                  Level2(type.Header) + HeaderSeparator +
                                  BulletPoint(Description(1)) +
                                  BulletPoint(Description(2)));
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


        [Fact]
        public void unordered_changelog_relevant_types_is_for_each_type_the_changelog_header_plus_a_group_containing_the_descriptions()
        {
            var commits = new[]
            {
                Conventional(Feature.Indicator, Description(1)),
                Conventional(Bugfix.Indicator, Description(2)),
                Conventional(PerformanceImprovement.Indicator, Description(3)),
                Conventional(Feature.Indicator, Description(4)),
                Conventional(PerformanceImprovement.Indicator, Description(5)),
                Conventional(Bugfix.Indicator, Description(6)),
            };
            var changelog = Changelog.From(commits);
            changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                                  Level2(Feature.Header) + HeaderSeparator +
                                  BulletPoint(Description(1)) +
                                  BulletPoint(Description(4)) + HeaderSeparator +
                                  Level2(Bugfix.Header) + HeaderSeparator +
                                  BulletPoint(Description(2)) +
                                  BulletPoint(Description(6)) + HeaderSeparator +
                                  Level2(PerformanceImprovement.Header) + HeaderSeparator +
                                  BulletPoint(Description(3)) +
                                  BulletPoint(Description(5)));
        }
    }
}
