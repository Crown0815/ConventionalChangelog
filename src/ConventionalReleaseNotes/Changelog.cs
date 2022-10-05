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
            var prefix = "feat: ";
            var header = "## Features";
            if (commitMessage?.Contains(prefix) is true)
            {
                if (!changelog.Contains(header))
                {
                    changelog += NewLine;
                    changelog += header + NewLine + NewLine;
                }
                changelog += commitMessage.Replace(prefix, BulletPoint);
            }

            prefix = "fix: ";
            header = "## Bug Fixes";
            if (commitMessage?.Contains(prefix) is true)
            {
                if (!changelog.Contains(header))
                {
                    changelog += NewLine;
                    changelog += header + NewLine + NewLine;
                }
                changelog += commitMessage.Replace(prefix, BulletPoint);
            }
        }
        return changelog;
    }
}
