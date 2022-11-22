using System.Text.RegularExpressions;
using static System.Environment;

namespace ConventionalReleaseNotes;

internal class LogAggregate
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private static readonly string EmptyChangelog = ChangelogTitle + NewLine;

    public string Text { get; set; } = EmptyChangelog;
    public bool HasGeneralCodeImprovements { get; set; }

    public bool IsEmpty => Text == EmptyChangelog;

    public void AddBullet(string text) => Text += BulletPoint + text + NewLine;

    public override string ToString()
    {
        if (IsEmpty && HasGeneralCodeImprovements)
            return Text + NewLine + "*General Code Improvements*";
        return Text;
    }
}


public static class Changelog
{
    private static readonly ConventionalCommitType[] CommitTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
        new("[a-z]+: ", "", true),
    };

    public static string From(params string[] o)
    {
        var log = new LogAggregate();

        foreach (var commitType in CommitTypes)
        foreach (var commitMessage in o)
        {
            if (commitMessage.DoesNotMatch(commitType)) continue;

            if (commitType.HideFromChangelog)
                log.HasGeneralCodeImprovements = true;
            else
            {
                if (!log.Text.Contains(commitType.Header))
                {
                    if (!log.IsEmpty)
                        log.Text += NewLine;

                    log.Text += NewLine;
                    log.Text += ChangeGroupHeader(commitType.Header) + NewLine + NewLine;
                }

                log.AddBullet(commitMessage.Replace(commitType.Indicator, ""));
            }
        }

        return log.ToString();
    }

    private static bool DoesNotMatch(this string m, ConventionalCommitType t) => !Regex.IsMatch(m, @$"{t.Indicator}.+");

    private static string ChangeGroupHeader(string header) => $"## {header}";
}
