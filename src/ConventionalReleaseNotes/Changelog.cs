using static System.Environment;

namespace ConventionalReleaseNotes;

public class Changelog
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";

    public static string From(params string?[] o)
    {
        var changelog = ChangelogTitle + NewLine;
        foreach (var commitMessage in o)
        {
            foreach (var change in new ChangeType[]{new("feat: ", "## Features"), new("fix: ", "## Bug Fixes")})
            {
                if (commitMessage?.Contains(change.Prefix) is not true) continue;

                if (!changelog.Contains(change.Header))
                {
                    changelog += NewLine;
                    changelog += change.Header + NewLine + NewLine;
                }
                changelog += commitMessage.Replace(change.Prefix, BulletPoint);
            }
        }
        return changelog;
    }

    private record ChangeType(string Prefix, string Header);
}
