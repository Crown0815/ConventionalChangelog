using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;
using static System.Text.RegularExpressions.RegexOptions;

namespace ConventionalChangelog;

public static class StringMatchingExtensions
{
    public static bool Matches(this string input, CommitType t) =>
        input.Matches(t.Indicator, None);

    public static bool Matches(this string pattern, CommitMessage.Footer f) =>
        f.Token.Matches(pattern, IgnoreCase);

    public static bool Matches(this string input, string pattern) =>
        input.Matches(pattern, None);

    private static bool Matches(this string input, string pattern, RegexOptions options) =>
        HasMatch(input, $"^{pattern}$", options);

    public static bool ContainsMatchFor(this string line, string pattern) =>
        HasMatch(line, pattern, None);

    private static bool HasMatch(string input, string pattern, RegexOptions options) =>
        Regex.IsMatch(input, pattern, options);
}
