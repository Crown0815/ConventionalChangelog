using System;
using System.Collections.Generic;
using System.Linq;

namespace ConventionalChangelog.Unit.Tests;

internal static class VariantsExtensions
{
    public static IEnumerable<string> CaseVariants(this string original) => AllVariants(original).Distinct();

    private static IEnumerable<string> AllVariants(string original)
    {
        yield return original.ToLower();
        yield return original.To(LowerUpper);
        yield return original.To(UpperLower);
        yield return original.ToUpper();
    }

    private static string To(this string original, Func<char, int, char> rule) => new(original.Select(rule).ToArray());

    private static char UpperLower(char @char, int index) => LowerOrUpperIfIndex(@char, index, 0);
    private static char LowerUpper(char @char, int index) => LowerOrUpperIfIndex(@char, index, 1);

    private static char LowerOrUpperIfIndex(char @char, int index, int offset)
    {
        return index % 2 == offset
            ? char.ToUpper(@char)
            : char.ToLower(@char);
    }
}
