using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalReleaseNotes.Unit.Tests.CommitType;

namespace ConventionalReleaseNotes.Unit.Tests.Changelog_specs;

public class A_changelog_from_changelog_relevant_conventional_commits
{
    public static readonly object[][] ChangelogRelevantCommitTypes =
    {
        new object[] { Feature },
        new object[] { Bugfix },
        new object[] { PerformanceImprovement },
    };

    private readonly Model.Changelog _changelog = Model.Changelog.Empty;

    [Theory]
    [MemberData(nameof(ChangelogRelevantCommitTypes))]
    public void is_the_changelog_header_plus_a_group_containing_the_descriptions(ConventionalCommitType type)
    {
        var message1 = type.CommitWithDescription(1);
        var message2 = type.CommitWithDescription(2);

        var changelog = Changelog.From(message1, message2);

        changelog.Should().Be(_changelog.WithGroup(type.Header, 1, 2));
    }

    [Fact]
    public void and_irrelevant_commits_contains_all_relevant_entries()
    {
        var message1 = Feature.CommitWithDescription(1);
        var message2 = "chore".ToCommitType().CommitWith(Model.Description(2));

        var changelog = Changelog.From(message1, message2);

        changelog.Should().Be(_changelog.WithGroup(Feature.Header, 1));
    }

    [Fact]
    public void in_random_order_is_for_each_type_the_changelog_header_plus_a_group_containing_the_descriptions()
    {
        var messages = new[]
        {
            Feature.CommitWithDescription(1),
            Bugfix.CommitWithDescription(2),
            PerformanceImprovement.CommitWithDescription(3),
            Feature.CommitWithDescription(4),
            PerformanceImprovement.CommitWithDescription(5),
            Bugfix.CommitWithDescription(6),
        };

        var changelog = Changelog.From(messages);

        changelog.Should().Be(_changelog
            .WithGroup(Feature.Header, 1, 4)
            .WithGroup(Bugfix.Header, 2, 6)
            .WithGroup(PerformanceImprovement.Header, 3, 5));
    }

    [Fact]
    public void with_breaking_change_type_contains_message_within_special_breaking_changes_group()
    {
        const string breakingChangesHeader = "Breaking Changes";
        var breakingChange = Breaking(Feature).CommitWithDescription(1);

        var changelog = Changelog.From(breakingChange);

        changelog.Should().Be(_changelog
            .WithGroup(breakingChangesHeader, 1));
    }

    [Fact]
    public void with_breaking_and_other_change_types_contains_breaking_changes_as_first_group()
    {
        const string breakingChangesHeader = "Breaking Changes";
        var breakingChange = Breaking(Feature).CommitWithDescription(1);
        var anotherChange = Feature.CommitWithDescription(2);

        var changelog = Changelog.From(breakingChange, anotherChange);

        changelog.Should().Be(_changelog
            .WithGroup(breakingChangesHeader, 1)
            .WithGroup(Feature.Header, 2));
    }

    [Theory]
    [InlineData("BREAKING CHANGE")]
    [InlineData("BREAKING-CHANGE")]
    public void contains_the_breaking_change_description_followed_by_the_commit_description_when_containing_a(string breakingChangeFooterToken)
    {
        const string breakingChangesHeader = "Breaking Changes";
        var breakingChange = Feature.CommitWithDescription(1);
        breakingChange += NewLine + NewLine + breakingChangeFooterToken + ": " + Model.Description(2);

        var changelog = Changelog.From(breakingChange);

        changelog.Should().Be(_changelog
            .WithGroup(breakingChangesHeader, 2)
            .WithGroup(Feature.Header, 1));
    }

    [Theory]
    [InlineData("BREAKING CHANGE")]
    [InlineData("BREAKING-CHANGE")]
    public void contains_the_breaking_change_description_followed_by_the_commit_description_when_containing_a_breaking_change_type_and_a(string breakingChangeFooterToken)
    {
        const string breakingChangesHeader = "Breaking Changes";
        var breakingChange = Breaking(Feature).CommitWithDescription(1);
        breakingChange += NewLine + NewLine + breakingChangeFooterToken + ": " + Model.Description(2);

        var changelog = Changelog.From(breakingChange);

        changelog.Should().Be(_changelog
            .WithGroup(breakingChangesHeader, 2)
            .WithGroup(Feature.Header, 1));
    }
}
