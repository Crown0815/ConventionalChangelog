using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_cli_program_when_given_changelog_order : CliTestsBase
{
    public static TheoryData<string> ChangelogOrderData => TheoryDataFrom(ChangelogOrderKeys);

    [Theory, MemberData(nameof(ChangelogOrderData))]
    public void newest_to_oldest_orders_changelog_entries_newest_to_oldest(string argument)
    {
        const string order = "NewestToOldest";
        Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2);

        var output = OutputWithInput($"{argument} {order} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 2, 1) + NewLine);
    }

    [Theory, MemberData(nameof(ChangelogOrderData))]
    public void oldest_to_newest_orders_changelog_oldest_to_newest(string argument)
    {
        const string order = "OldestToNewest";
        Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2);

        var output = OutputWithInput($"{argument} {order} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 1, 2) + NewLine);
    }
}
