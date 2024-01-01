using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;
using static System.Text.RegularExpressions.RegexOptions;

namespace ConventionalChangelog;

public static class StringMatchingExtensions
{
    public static bool Matches(this string input, CommitType type) =>
        input.FullyMatches(type.Indicator, None);

    public static bool Matches(this string pattern, CommitMessage.Footer footer) =>
        footer.Token.FullyMatches(pattern, IgnoreCase);

    public static bool Matches(this string input, string pattern) =>
        input.FullyMatches(pattern, None);

    private static bool FullyMatches(this string input, string pattern, RegexOptions options) =>
        IsMatch(input, $"^{pattern}$", options);

    public static bool StartMatches(this string input, string pattern) =>
        IsMatch(input, $"^{pattern}", None);

    private static bool IsMatch(string input, string pattern, RegexOptions options) =>
        Regex.IsMatch(input, pattern, options);

    public static Match MatchWith(this string input, string pattern) => Regex.Match(input, pattern);

    public static MatchCollection MatchesWith(this string input, string pattern) => Regex.Matches(input, pattern);

    public static string ReplaceWith(this string input, string pattern, string group)
    {
        return Regex.Replace(input, pattern, x => x.Groups[group].Value);
    }
}
