using Xunit;
using System.Diagnostics;
using FluentAssertions;
using static System.Environment;
using static ConventionalReleaseNotes.Unit.Tests.CommitTypeFor;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Command_line_interface_specs : GitUsingTestsBase
{
    [Fact]
    public void f()
    {
        Repository.Commit(Feature, Model.Description(1));

        using var process = new Process();

        process.StartInfo.FileName = @$".\{nameof(ConventionalReleaseNotes)}.exe";
        process.StartInfo.Arguments = Repository.Path();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        output.Should().Be(Model.Changelog.Empty.WithGroup(Feature, 1) + NewLine);
    }
}
