using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_cli_program : CliTestsBase
{
    [Fact]
    public void prints_the_changelog_from_a_given_repository_to_the_console()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path());

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }

    [Fact]
    public void run_from_teamcity_prints_a_service_message_setting_a_parameter_to_the_changelog()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path(), (TeamCity.EnvironmentVariable, "whatever"));

        output.Should().Be(TeamCity.SetParameterCommand("CRN.Changelog", A.Changelog.WithGroup(Feature, 1)) + NewLine);
    }
}
