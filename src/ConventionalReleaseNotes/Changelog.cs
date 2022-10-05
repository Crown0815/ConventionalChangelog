using static System.Environment;

namespace ConventionalReleaseNotes;

public class Changelog
{
    public static string From(string? o)
    {
        var changelog = "# Changelog" + NewLine;
        if (o?.Contains("feat: ") is true)
        {
            changelog += NewLine;
            changelog += "## Features" + NewLine + NewLine;
            changelog += o.Replace("feat: ", "- ");
        }
        return changelog;
    }
}
