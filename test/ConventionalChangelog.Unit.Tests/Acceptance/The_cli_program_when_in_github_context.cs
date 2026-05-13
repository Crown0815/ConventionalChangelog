using System.IO;
using ConventionalChangelog.BuildSystems;
using AwesomeAssertions;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_in_github_context : CliTestsBase
{
    private readonly string _githubOutputFileName = Path.GetTempFileName();

    [Fact]
    public void prints_an_output_command_setting_a_parameter_to_the_changelog()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(
            Repository.Path(),
            (GitHub.EnvironmentVariable, "true"),
            (GitHub.EnvironmentOutputVariable, _githubOutputFileName));

        output.Should().BeEmpty();

        File.ReadAllText(_githubOutputFileName).Should().Be($"""
            {GitHub.GenerateContent(Output.ChangelogLegacy, A.Changelog.WithGroup(Feature, 1))}
            {GitHub.GenerateContent(Output.Changelog, A.Changelog.WithGroup(Feature, 1))}

            """);
    }

    public override void Dispose()
    {
        File.Delete(_githubOutputFileName);
        base.Dispose();
    }
}
