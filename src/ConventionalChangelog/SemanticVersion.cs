using System.Text.RegularExpressions;

namespace ConventionalChangelog;

internal static class SemanticVersion
{
    private static readonly Regex Regex = new(@"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$");

    public static bool IsSemanticVersion(this string text, string prefix) => Regex.IsMatch("^"+prefix+text);
}
