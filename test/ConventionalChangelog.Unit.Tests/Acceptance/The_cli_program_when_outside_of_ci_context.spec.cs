using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_cli_program_when_outside_of_ci_context : CliTestsBase
{
    [Fact]
    public void prints_the_changelog_from_a_given_repository_to_the_console()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path());

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }
}
