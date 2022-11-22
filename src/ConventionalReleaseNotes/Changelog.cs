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

    public override string ToString()
    {
        if (IsEmpty && HasGeneralCodeImprovements)
            return Text + NewLine + "*General Code Improvements*";
        return Text;
    }
}


public class Changelog
{
    private static readonly ConventionalCommitType[] ChangeTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
        new("[a-z]+: ", "", false),
    };


    private const string BulletPoint = "- ";

    public static string From(params string[] o)
    {
        var log = new LogAggregate();

        foreach (var change in ChangeTypes)
        {
            foreach (var commitMessage in o)
            {
                if (!Regex.IsMatch(commitMessage, @$"{change.Indicator}.+")) continue;

                if (change.ShallDisplay)
                {
                    if (!log.Text.Contains(change.Header))
                    {
                        if (!log.IsEmpty)
                            log.Text += NewLine;

                        log.Text += NewLine;
                        log.Text += ChangeGroupHeader(change.Header) + NewLine + NewLine;
                    }
                    log.Text += commitMessage.Replace(change.Indicator, BulletPoint) + NewLine;
                }
                else
                {
                    log.HasGeneralCodeImprovements = true;
                }
            }
        }
        return log.ToString();
    }

    private static string ChangeGroupHeader(string header) => $"## {header}";
}
