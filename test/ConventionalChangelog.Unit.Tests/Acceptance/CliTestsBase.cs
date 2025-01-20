using System;
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
    protected static readonly string[] IgnoreScope = ["-s", "--ignore-scope"];

    protected static string OutputWithInput(string arguments, params (string, string)[] environmentVariables)
    {
        using var process = new Process();

        process.StartInfo.FileName = Path.Join(".",nameof(ConventionalChangelog));
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;
        foreach (var (name, value) in environmentVariables)
            process.StartInfo.EnvironmentVariables[name] = value;

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new ApplicationException($"Process failed with exit code {process.ExitCode}: {error}");

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
