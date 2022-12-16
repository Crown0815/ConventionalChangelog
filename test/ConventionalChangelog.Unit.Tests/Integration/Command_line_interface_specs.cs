using System.Collections;
using System.Diagnostics;
using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Integration;

public class Command_line_interface_specs : GitUsingTestsBase
{
    private static string OutputWithInput(string repositoryPath, params (string, string)[] environmentVariables)
    {
        using var process = new Process();

        process.StartInfo.FileName = @$".\{nameof(ConventionalChangelog)}.exe";
        process.StartInfo.Arguments = repositoryPath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;
        foreach (var (name, value) in environmentVariables)
            process.StartInfo.EnvironmentVariables[name] = value;

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }

    [Fact]
    public void The_program_prints_the_changelog_from_a_given_repository()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path());

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }

    [Fact]
    public void The_program_run_from_teamcity_prints_a_service_message_setting_a_parameter_to_the_changelog()
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput(Repository.Path(), (TeamCity.EnvironmentVariable, "whatever"));

        output.Should().Be(TeamCity.SetParameterCommand("CRN.Changelog", A.Changelog.WithGroup(Feature, 1)) + NewLine);
    }
}
