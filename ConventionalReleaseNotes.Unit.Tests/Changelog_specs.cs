using Shouldly;
using Xunit;
using static System.Environment;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Changelog_specs
{
    private const string ChangelogHeader = "# Changelog";
    private const string FeaturesHeader = "## Features";
    private const string BulletPoint = "- ";
    private const string BugFixesHeader = "## Bug Fixes";

    private static readonly string EmptyChangeLog = ChangelogHeader + NewLine;
    private static readonly string HeaderSeparator = NewLine + NewLine;

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void A_changelog_with_no_changes_is_just_the_changelog_header(string noChanges)
    {
        var changelog = Changelog.From(noChanges);
        changelog.ShouldBe(EmptyChangeLog);
    }

    [Theory]
    [InlineData("some message")]
    [InlineData("1234: abc")]
    public void A_changelog_from_non_conventional_commits_is_just_the_changelog_header(string nonConventionalCommits)
    {
        var changelog = Changelog.From(nonConventionalCommits);
        changelog.ShouldBe(EmptyChangeLog);
    }

    [Fact]
    public void A_changelog_from_a_feature_is_the_changelog_header_plus_a_feature_group_containing_the_feature()
    {
        var featureCommit = Feature("New Feature");
        var changelog = Changelog.From(featureCommit);
        changelog.ShouldBe(ChangelogHeader + HeaderSeparator +
                           FeaturesHeader + HeaderSeparator +
                           BulletPoint + "New Feature");
    }

    [Fact]
    public void A_changelog_from_multiple_features_is_the_changelog_header_plus_a_feature_group_containing_the_features()
    {
        var featureCommit1 = Feature("New Feature1");
        var featureCommit2 = Feature("New Feature2");
        var changelog = Changelog.From(featureCommit1, featureCommit2);
        changelog.ShouldBe(ChangelogHeader + HeaderSeparator +
                           FeaturesHeader + HeaderSeparator +
                           BulletPoint + "New Feature1" +
                           BulletPoint + "New Feature2");
    }

    [Fact]
    public void A_changelog_from_a_fix_is_the_changelog_header_plus_a_feature_group_containing_the_feature()
    {
        var featureCommit = Bugfix("New Fix");
        var changelog = Changelog.From(featureCommit);
        changelog.ShouldBe(ChangelogHeader + HeaderSeparator +
                           BugFixesHeader + HeaderSeparator +
                           BulletPoint + "New Fix");
    }

    private static string Feature(string summary) => "feat: " + summary;
    private static string Bugfix(string summary) => "fix: " + summary;
}
