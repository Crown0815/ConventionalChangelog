using System.Diagnostics;
using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalReleaseNotes.Unit.Tests.CommitTypeFor;

namespace ConventionalReleaseNotes.Unit.Tests.Integration;

public class Command_line_interface_specs : GitUsingTestsBase
{
    private static string OutputWithInput(string repositoryPath)
    {
        using var process = new Process();

        process.StartInfo.FileName = @$".\{nameof(ConventionalReleaseNotes)}.exe";
        process.StartInfo.Arguments = repositoryPath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }

    [Fact]
    public void The_program_prints_the_changelog_from_a_given_repository()
    {
        Repository.CommitWithDescription(Feature, 1);

        var output = OutputWithInput(Repository.Path());

        output.Should().Be(Model.Changelog.WithGroup(Feature, 1) + NewLine);
    }

    [Fact]
    public void The_program_run_from_teamcity_prints_a_service_message_setting_a_parameter_to_the_changelog()
    {
        Repository.CommitWithDescription(Feature, 1);
        SetEnvironmentVariable(TeamCity.EnvironmentVariable, "whatever");

        var output = OutputWithInput(Repository.Path());

        output.Should().Be(TeamCity.SetParameterCommand("CRN.Changelog", Model.Changelog.WithGroup(Feature, 1)) + NewLine);
    }
}
