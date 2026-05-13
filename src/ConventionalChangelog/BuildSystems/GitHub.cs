using System.Text;

namespace ConventionalChangelog.BuildSystems;

public static class GitHub
{
    // see https://docs.github.com/en/actions/writing-workflows/choosing-what-your-workflow-does/store-information-in-variables#default-environment-variables
    // The GITHUB_ACTIONS environment variable is always set to true when GitHub Actions is running the workflow
    public const string EnvironmentVariable = "GITHUB_ACTIONS";

    // see https://docs.github.com/en/actions/writing-workflows/choosing-what-your-workflow-does/workflow-commands-for-github-actions#setting-an-output-parameter
    public static string GenerateContent(string name, string value)
    {
        return $"{name}<<EOF{Environment.NewLine}{Escaped(value)}{Environment.NewLine}EOF";
    }

    public static bool IsCurrentCi()
    {
        return Environment.GetEnvironmentVariable(EnvironmentVariable) is not null;
    }

    private static string Escaped(string raw) => raw
        .ReplaceUniCode();

    private static string ReplaceUniCode(this string raw) =>
        raw.Aggregate(new StringBuilder(), Append).ToString();

    private static StringBuilder Append(StringBuilder b, char character) =>
        b.Append(Escaped(character));

    private static object Escaped(char c) => char.IsAscii(c)
        ? c
        : Escaped((int)c);

    private static string Escaped(int c) => "\\u" + c.ToString("x4");

}
