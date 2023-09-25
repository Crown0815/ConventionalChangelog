using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace ConventionalChangelog.Unit.Tests;


internal class CaseVariantDataAttribute: DataAttribute
{
    private readonly string _source;

    public CaseVariantDataAttribute(string source) => _source = source;

    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => _source.TestCases();
}

internal static class VariantsExtensions
{
    public static IEnumerable<object[]> TestCases(this string source)
    {
        return CaseVariantsFrom(source).Select(x => new object[] { x });
    }

    private static IEnumerable<string> CaseVariantsFrom(string original) =>
        AllVariants(original).Reverse().Append(original).Distinct().Reverse();

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
