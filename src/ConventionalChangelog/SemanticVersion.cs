using System.Text.RegularExpressions;

namespace ConventionalChangelog;

#if NET6_0
internal static class SemanticVersion
{
    private static Regex SemanticVersionRegex() => new(SemanticVersionPattern);
#elif NET7_0_OR_GREATER
internal static partial class SemanticVersion
{
    [GeneratedRegex(SemanticVersionPattern)]
    private static partial Regex SemanticVersionRegex();
#endif

    // language=regex
    private const string SemanticVersionPattern = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$";

    private static readonly Lazy<Regex> Regex = new(SemanticVersionRegex);

    public static bool IsSemanticVersion(this string text, string prefix) => Regex.Value.IsMatch(prefix + text);
}
