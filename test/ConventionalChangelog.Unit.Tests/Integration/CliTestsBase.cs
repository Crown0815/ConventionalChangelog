using System.Diagnostics;
using System.IO;

namespace ConventionalChangelog.Unit.Tests.Integration;

public abstract class CliTestsBase : GitUsingTestsBase
{
    protected string OutputWithInput(string arguments, params (string, string)[] environmentVariables)
    {
        using var process = new Process();

        process.StartInfo.FileName = Path.Join(".",nameof(ConventionalChangelog));
        process.StartInfo.Arguments = arguments;
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
}
