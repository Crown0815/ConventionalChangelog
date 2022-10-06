using static System.Environment;

namespace ConventionalReleaseNotes;

public class Changelog
{
    private static readonly ChangeType[] ChangeTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
    };

    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";

    public static string From(params string[] o)
    {
        var changelog = ChangelogTitle + NewLine;
        foreach (var commitMessage in o)
        {
            foreach (var change in ChangeTypes)
            {
                if (commitMessage.Contains(change.Prefix) is not true) continue;

                if (!changelog.Contains(change.Header))
                {
                    changelog += NewLine;
                    changelog += ChangeGroupHeader(change.Header) + NewLine + NewLine;
                }
                changelog += commitMessage.Replace(change.Prefix, BulletPoint);
            }
        }
        return changelog;
    }

    private static string ChangeGroupHeader(string header) => $"## {header}";

    private record ChangeType(string Prefix, string Header);
}
