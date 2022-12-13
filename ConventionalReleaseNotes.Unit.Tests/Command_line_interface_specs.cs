using Xunit;
using System.Diagnostics;
using FluentAssertions;
using static System.Environment;
using static ConventionalReleaseNotes.Unit.Tests.CommitTypeFor;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Command_line_interface_specs : GitUsingTestsBase
{
    private static string ChangelogFrom(string repositoryPath)
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
    public void The_program_can_generate_a_changelog_from_a_given_repository()
    {
        Repository.Commit(Feature, Model.Description(1));

        var changelog = ChangelogFrom(Repository.Path());

        changelog.Should().Be(Model.Changelog.Empty.WithGroup(Feature, 1) + NewLine);
    }
}
