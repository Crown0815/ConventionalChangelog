using System.Text.RegularExpressions;

namespace ConventionalChangelog;

internal static partial class SemanticVersion
{
    // language=regex
    private const string SemanticVersionPattern = @"([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$";

#if NET6_0
    private static Regex SemanticVersionRegex() => new(SemanticVersionPattern);
#elif NET7_0_OR_GREATER
    [GeneratedRegex(SemanticVersionPattern)]
    private static partial Regex SemanticVersionRegex();
#endif

    private static readonly Lazy<Regex> Regex = new(SemanticVersionRegex);

    public static bool IsSemanticVersion(this string text, string prefix) => Regex.Value.IsMatch(prefix + text);
}
