using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_include_hashes_flag : CliTestsBase
{
    public static TheoryData<string> ShowHashesData { get; } = TheoryDataFrom(ShowHash);

    [Theory, MemberData(nameof(ShowHashesData))]
    public void includes_the_sha_from_changelog_entry_source_in_the_output(string argument)
    {
        var commit = Repository.Commit(Feature, A.Description(1));

        var output = OutputWithInput($"{argument} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, (1, commit.Sha)) + NewLine);
    }
}
