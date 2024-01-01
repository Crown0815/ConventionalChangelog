using FluentAssertions;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public partial class A_changelog_from_changelog_relevant_conventional_commits
{
    public static readonly TheoryData<CommitType> ChangelogRelevantCommitTypes = new()
    {
        Feature,
        Bugfix,
        PerformanceImprovement,
    };

    [Theory]
    [MemberData(nameof(ChangelogRelevantCommitTypes))]
    public void is_the_changelog_header_plus_a_group_containing_the_descriptions(CommitType type)
    {
        var message1 = type.CommitWithDescription(1);
        var message2 = type.CommitWithDescription(2);

        var changelog = A.Changelog.From(message1, message2);

        changelog.Should().Be(A.Changelog.WithGroup(type, 1, 2));
    }

    [Fact]
    public void and_irrelevant_commits_contains_all_relevant_entries()
    {
        var message1 = Feature.CommitWithDescription(1);
        var message2 = Irrelevant.CommitWithDescription(2);

        var changelog = A.Changelog.From(message1, message2);

        changelog.Should().Be(A.Changelog.WithGroup(Feature, 1));
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

        var changelog = A.Changelog.From(messages);

        changelog.Should().Be(A.Changelog
            .WithGroup(Feature, 1, 4)
            .And(Bugfix, 2, 6)
            .And(PerformanceImprovement, 3, 5));
    }
}
