using System.Diagnostics;
using LibGit2Sharp;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public static class RepositoryDebugExtensions
{
    internal static string LogGraph(this Repository repository)
    {
        using var process = new Process();

        process.StartInfo.FileName = "git";
        process.StartInfo.Arguments = $"-C {repository.Path()} log --graph --all --oneline --decorate";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }
}
