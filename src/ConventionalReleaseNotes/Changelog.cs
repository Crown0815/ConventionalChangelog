namespace ConventionalReleaseNotes;

public class Changelog
{
    public static string From(string? o)
    {
        var changelog = "# Changelog" + Environment.NewLine;
        if (o?.Contains("feat: ") is true)
        {
            changelog += "## Features" + Environment.NewLine + Environment.NewLine;
            changelog += o.Replace("feat: ", "- ");
        }
        return changelog;
    }
}
