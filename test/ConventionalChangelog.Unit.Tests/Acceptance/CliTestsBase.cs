using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public abstract class CliTestsBase : GitUsingTestsBase
{
    protected static readonly string[] OutputKeys = ["-o", "--output"];
    public static readonly string[] TagPrefixKeys = ["-t", "--tag-prefix"];
    protected static readonly string[] IgnorePrerelease = ["-i", "--ignore-prereleases"];

    protected static string OutputWithInput(string arguments, params (string, string)[] environmentVariables)
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

    protected static TheoryData<T> TheoryDataFrom<T>(IEnumerable<T> values)
    {
        return values.Aggregate(new TheoryData<T>(), (theoryData, value) =>
        {
            theoryData.Add(value);
            return theoryData;
        });
    }
}
