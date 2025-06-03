using ConventionalChangelog.BuildSystems;
using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_cli_program_when_in_teamcity_context : CliTestsBase
{
    [Fact]
    public void prints_a_service_message_setting_a_parameter_to_the_changelog()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path(), (TeamCity.EnvironmentVariable, "whatever"));

        output.Should().Be(TeamCity.SetParameterCommand(Output.Changelog, A.Changelog.WithGroup(Feature, 1)) + NewLine);
    }
}
