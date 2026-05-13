using ConventionalChangelog.BuildSystems;
using AwesomeAssertions;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_cli_program_when_in_github_context : CliTestsBase
{
    [Fact]
    public void prints_an_output_command_setting_a_parameter_to_the_changelog()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path(), (GitHub.EnvironmentVariable, "true"));

        output.Should().Be($"""
                            {GitHub.SetOutputCommand(Output.ChangelogLegacy, A.Changelog.WithGroup(Feature, 1))}
                            {GitHub.SetOutputCommand(Output.Changelog, A.Changelog.WithGroup(Feature, 1))}

                            """);
    }
}
