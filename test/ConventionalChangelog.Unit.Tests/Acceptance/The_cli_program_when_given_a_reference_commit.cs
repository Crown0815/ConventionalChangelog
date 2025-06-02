using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_cli_program_when_given_a_reference_commit : CliTestsBase
{
    public static TheoryData<string> ReferenceCommitData => TheoryDataFrom(ReferenceCommitKeys);

    [Theory, MemberData(nameof(ReferenceCommitData))]
    public void prints_changelog_with_all_commits_since_reference_commit(string argument)
    {
        var reference = Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2);
        Repository.Commit(Feature, 3);

        var output = OutputWithInput($"{argument} {reference.Sha} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 3, 2) + NewLine);
    }
}
