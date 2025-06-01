using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public abstract class CliTestsBase : GitUsingTestsBase
{
    protected static readonly string[] OutputKeys = ["-o", "--output"];
    public static readonly string[] TagPrefixKeys = ["-t", "--tag-prefix"];
    protected static readonly string[] ChangelogOrderKeys = ["-c", "--changelog-order"];
    protected static readonly string[] IgnorePrerelease = ["-i", "--ignore-prereleases"];
    protected static readonly string[] SkipTitleFlag = ["-r", "--skip-title"];
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
        if (process.StartInfo.EnvironmentVariables.ContainsKey("DOTNET_ROOT") == false)
            process.StartInfo.EnvironmentVariables["DOTNET_ROOT"] = Path.GetDirectoryName(Environment.ProcessPath);

        foreach (var (name, value) in environmentVariables)
            process.StartInfo.EnvironmentVariables[name] = value;

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
            ProvideDebugInformationAndThrow(process, error);

        return output;
    }

    private static void ProvideDebugInformationAndThrow(Process process, string error)
    {
        var writer = new StringBuilder();
        writer.AppendLine($"Process failed with exit code {process.ExitCode}");
        writer.AppendLine(error);
        writer.AppendLine();
        writer.AppendLine("EnvironmentVariables:");
        writer.AppendLine(EnvironmentVariables(process));
        writer.AppendLine();
        writer.AppendLine("Other values:");
        writer.AppendLine($"Environment.ProcessPath: {Environment.ProcessPath}");

        throw new ApplicationException(writer.ToString());
    }

    private static string EnvironmentVariables(Process process)
    {
        return string.Join(Environment.NewLine, EnvironmentVariablesLines(process));
    }

    private static IEnumerable<string> EnvironmentVariablesLines(Process process)
    {
        foreach (string key in process.StartInfo.EnvironmentVariables.Keys)
            yield return $"- {key}: {process.StartInfo.EnvironmentVariables[key]}";
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
