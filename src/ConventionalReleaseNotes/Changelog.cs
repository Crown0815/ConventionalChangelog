using System.Text.RegularExpressions;
using static System.Environment;

namespace ConventionalReleaseNotes;

internal class LogAggregate
{
    private const string ChangelogTitle = "# Changelog";
    private static readonly string EmptyChangelog = ChangelogTitle + NewLine;

    public string Text { get; set; } = EmptyChangelog;
    public bool HasGeneralCodeImprovements { get; set; }

    public bool IsEmpty => Text == EmptyChangelog;
}


public class Changelog
{
    private static readonly ConventionalCommitType[] ChangeTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
    };


    private const string BulletPoint = "- ";

    public static string From(params string[] o)
    {
        var log = new LogAggregate();

        foreach (var change in ChangeTypes)
        {
            foreach (var commitMessage in o)
            {
                if (!log.HasGeneralCodeImprovements && Regex.IsMatch(commitMessage, "[a-z]+: .+"))
                    log.HasGeneralCodeImprovements = true;

                if (!commitMessage.Contains(change.Indicator)) continue;

                if (!log.Text.Contains(change.Header))
                {
                    if (!log.IsEmpty)
                        log.Text += NewLine;

                    log.Text += NewLine;
                    log.Text += ChangeGroupHeader(change.Header) + NewLine + NewLine;
                }
                log.Text += commitMessage.Replace(change.Indicator, BulletPoint) + NewLine;
            }
        }
        if (log.IsEmpty && log.HasGeneralCodeImprovements)
            log.Text += NewLine + "*General Code Improvements*";
        return log.Text;
    }

    private static string ChangeGroupHeader(string header) => $"## {header}";
}
