using FluentAssertions;
using Xunit;
using static ConventionalReleaseNotes.Unit.Tests.CommitType;

namespace ConventionalReleaseNotes.Unit.Tests.Changelog_specs;

public class A_changelog_from_relevant_conventional_commits
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
        var message1 = Model.ConventionalCommitMessage(type, Model.Message(1));
        var message2 = Model.ConventionalCommitMessage(type, Model.Message(2));

        var changelog = Changelog.From(message1, message2);

        changelog.Should().Be(_changelog
            .WithGroup(type.Header)
                .WithBullet(Model.Message(1))
                .WithBullet(Model.Message(2)));
    }

    [Fact]
    public void and_irrelevant_commits_contains_all_relevant_entries()
    {
        var message1 = Model.ConventionalCommitMessage(Feature, Model.Message(1));
        var message2 = Model.ConventionalCommitMessage("chore", Model.Message(2));

        var changelog = Changelog.From(message1, message2);

        changelog.Should().Be(_changelog
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(1)));
    }

    [Fact]
    public void in_random_order_is_for_each_type_the_changelog_header_plus_a_group_containing_the_descriptions()
    {
        var messages = new[]
        {
            Model.ConventionalCommitMessage(Feature, Model.Message(1)),
            Model.ConventionalCommitMessage(Bugfix, Model.Message(2)),
            Model.ConventionalCommitMessage(PerformanceImprovement, Model.Message(3)),
            Model.ConventionalCommitMessage(Feature, Model.Message(4)),
            Model.ConventionalCommitMessage(PerformanceImprovement, Model.Message(5)),
            Model.ConventionalCommitMessage(Bugfix, Model.Message(6)),
        };

        var changelog = Changelog.From(messages);

        changelog.Should().Be(_changelog
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(1))
                .WithBullet(Model.Message(4))
            .WithGroup(Bugfix.Header)
                .WithBullet(Model.Message(2))
                .WithBullet(Model.Message(6))
            .WithGroup(PerformanceImprovement.Header)
                .WithBullet(Model.Message(3))
                .WithBullet(Model.Message(5)));
    }

    [Fact]
    public void with_breaking_change_indicator_contains_message_within_special_breaking_changes_group()
    {
        const string breakingChangeIndicator = "!";
        var breakingChange = Model.ConventionalCommitMessage(Feature.Indicator + breakingChangeIndicator, Model.Message(1));

        var changelog = Changelog.From(breakingChange);

        changelog.Should().Be(_changelog
            .WithGroup("Breaking Changes")
            .WithBullet(Model.Message(1)));
    }
}
