using static System.Environment;

namespace ConventionalReleaseNotes;

public class Changelog
{
    public static string From(params string?[] o)
    {
        var changelog = "# Changelog" + NewLine;
        foreach (var commitMessage in o)
        {
            if (commitMessage?.Contains("feat: ") is true)
            {
                if (!changelog.Contains("## Features"))
                {
                    changelog += NewLine;
                    changelog += "## Features" + NewLine + NewLine;
                }
                changelog += commitMessage.Replace("feat: ", "- ");
            }
        }
        return changelog;
    }
}
