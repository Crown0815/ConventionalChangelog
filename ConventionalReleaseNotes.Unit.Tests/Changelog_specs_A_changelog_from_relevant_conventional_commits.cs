using FluentAssertions;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests;

public partial class Changelog_specs
{
    public class A_changelog_from_relevant_conventional_commits
    {
        private static string Description(int index) => $"Some Description{index}";

        private static readonly ConventionalCommitType Feature = new("feat", "Features");
        private static readonly ConventionalCommitType Bugfix = new("fix", "Bug Fixes");
        private static readonly ConventionalCommitType PerformanceImprovement = new("perf", "Performance Improvements");

        public static readonly object[][] ChangelogRelevantCommitTypes =
        {
            new object[] { Feature },
            new object[] { Bugfix },
            new object[] { PerformanceImprovement },
        };

        private readonly Model.Changelog _changelog = Model.Changelog.Empty;

        [Theory]
        [MemberData(nameof(ChangelogRelevantCommitTypes))]
        public void is_the_changelog_header_plus_a_group_containing_the_descriptions(
            ConventionalCommitType type)
        {
            var message1 = Model.ConventionalCommitMessage(type.Indicator, Description(1));
            var message2 = Model.ConventionalCommitMessage(type.Indicator, Description(2));

            var changelog = Changelog.From(message1, message2);

            changelog.Should().Be(_changelog
                .WithGroup(type.Header)
                    .WithBullet(Description(1))
                    .WithBullet(Description(2)));
        }

        [Fact]
        public void and_irrelevant_commits_contains_all_relevant_entries()
        {
            var message1 = Model.ConventionalCommitMessage(Feature.Indicator, Description(1));
            var message2 = Model.ConventionalCommitMessage("chore", Description(2));

            var changelog = Changelog.From(message1, message2);

            changelog.Should().Be(_changelog
                .WithGroup(Feature.Header)
                    .WithBullet(Description(1)));
        }

        [Fact]
        public void in_random_order_is_for_each_type_the_changelog_header_plus_a_group_containing_the_descriptions()
        {
            var messages = new[]
            {
                Model.ConventionalCommitMessage(Feature.Indicator, Description(1)),
                Model.ConventionalCommitMessage(Bugfix.Indicator, Description(2)),
                Model.ConventionalCommitMessage(PerformanceImprovement.Indicator, Description(3)),
                Model.ConventionalCommitMessage(Feature.Indicator, Description(4)),
                Model.ConventionalCommitMessage(PerformanceImprovement.Indicator, Description(5)),
                Model.ConventionalCommitMessage(Bugfix.Indicator, Description(6)),
            };

            var changelog = Changelog.From(messages);

            changelog.Should().Be(_changelog
                .WithGroup(Feature.Header)
                    .WithBullet(Description(1))
                    .WithBullet(Description(4))
                .WithGroup(Bugfix.Header)
                    .WithBullet(Description(2))
                    .WithBullet(Description(6))
                .WithGroup(PerformanceImprovement.Header)
                    .WithBullet(Description(3))
                    .WithBullet(Description(5)));
        }
    }
}
