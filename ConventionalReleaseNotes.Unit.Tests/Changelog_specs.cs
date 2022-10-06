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
    public void A_changelog_from_no_changes_is_empty(params string[] noChanges)
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


    private const string FeaturesHeader = "## Features";
    private const string BugFixesHeader = "## Bug Fixes";
    private const string PerformanceImprovementsHeader = "## Performance Improvements";

    private static string Feature(string summary) => Conventional("feat", summary);
    private static string Bugfix(string summary) => Conventional("fix", summary);
    private static string PerformanceImprovement(string summary) => Conventional("perf", summary);

    private static string Conventional(string type, string summary) => type + ": " + summary;

    [Fact]
    public void A_changelog_from_a_feature_is_the_changelog_header_plus_a_feature_group_containing_the_feature()
    {
        var featureCommit = Feature("New Feature");
        var changelog = Changelog.From(featureCommit);
        changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                              FeaturesHeader + HeaderSeparator +
                              BulletPoint + "New Feature");
    }

    [Fact]
    public void
        A_changelog_from_multiple_features_is_the_changelog_header_plus_a_feature_group_containing_the_features()
    {
        var featureCommit1 = Feature("New Feature1");
        var featureCommit2 = Feature("New Feature2");
        var changelog = Changelog.From(featureCommit1, featureCommit2);
        changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                              FeaturesHeader + HeaderSeparator +
                              BulletPoint + "New Feature1" +
                              BulletPoint + "New Feature2");
    }

    [Fact]
    public void A_changelog_from_a_fix_is_the_changelog_header_plus_a_feature_group_containing_the_feature()
    {
        var featureCommit = Bugfix("New Fix");
        var changelog = Changelog.From(featureCommit);
        changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                              BugFixesHeader + HeaderSeparator +
                              BulletPoint + "New Fix");
    }

    [Fact]
    public void A_changelog_from_a_perf_is_the_changelog_header_plus_a_performance_improvements_group()
    {
        var performanceImprovement = PerformanceImprovement("New Performance Improvement");
        var changelog = Changelog.From(performanceImprovement);
        changelog.Should().Be(ChangelogHeader + HeaderSeparator +
                              PerformanceImprovementsHeader + HeaderSeparator +
                              BulletPoint + "New Performance Improvement");
    }
}
