using System.Text.RegularExpressions;
using static System.Environment;

namespace ConventionalReleaseNotes;

public class Changelog
{
    private static readonly ConventionalCommitType[] ChangeTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
    };


    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private static readonly string EmptyChangelog = ChangelogTitle + NewLine;

    public static string From(params string[] o)
    {
        var changelog = EmptyChangelog;
        foreach (var commitMessage in o)
        {
            foreach (var change in ChangeTypes)
            {
                if (!commitMessage.Contains(change.Indicator)) continue;

                if (!changelog.Contains(change.Header))
                {
                    changelog += NewLine;
                    changelog += ChangeGroupHeader(change.Header) + NewLine + NewLine;
                }
                changelog += commitMessage.Replace(change.Indicator, BulletPoint);
            }
            if (Regex.IsMatch(commitMessage, "[a-z]+: .+"))
                if (changelog == EmptyChangelog)
                    changelog += NewLine + "*General Code Improvements*";
        }
        return changelog;
    }

    private static string ChangeGroupHeader(string header) => $"## {header}";
}
