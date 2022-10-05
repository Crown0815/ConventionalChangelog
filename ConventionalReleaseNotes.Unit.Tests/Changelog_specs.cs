using System;
using Shouldly;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Changelog_specs
{
    private const string ChangelogHeader = "# Changelog";
    private const string FeaturesHeader = "## Features";
    private const string BulletPoint = "- ";

    private static readonly string EmptyChangeLog = ChangelogHeader + Environment.NewLine;

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
        const string featureCommit = "feat: New Feature";
        var changelog = Changelog.From(featureCommit);
        changelog.ShouldBe(ChangelogHeader+Environment.NewLine+Environment.NewLine+
                           FeaturesHeader+Environment.NewLine+Environment.NewLine+
                           BulletPoint+"New Feature");
    }

    [Fact]
    public void A_changelog_from_multiple_features_is_the_changelog_header_plus_a_feature_group_containing_the_features()
    {
        const string featureCommit1 = "feat: New Feature1";
        const string featureCommit2 = "feat: New Feature2";
        var changelog = Changelog.From(featureCommit1, featureCommit2);
        changelog.ShouldBe(ChangelogHeader+Environment.NewLine+Environment.NewLine+
                           FeaturesHeader+Environment.NewLine+Environment.NewLine+
                           BulletPoint+"New Feature1"+
                           BulletPoint+"New Feature2");
    }
}
