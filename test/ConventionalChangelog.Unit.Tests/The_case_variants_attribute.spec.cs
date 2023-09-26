using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests;

public class The_case_variants_attribute
{
    private static IEnumerable<object[]> AttributeCasesFrom(string source)
    {
        return new CaseVariantDataAttribute(source).GetData(default!);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("0")]
    [InlineData("1234567890")]
    [InlineData("{")]
    [InlineData("[")]
    [InlineData("`")]
    [InlineData("@")]
    public void generates_xunit_test_cases_with_the_given_strings_for_numbers_and_special_characters(string input)
    {
        AttributeCasesFrom(input).Should()
            .ContainSingle().Which.Should()
            .ContainSingle().Which.Should().Be(input);
    }

    [Theory, SuppressMessage("ReSharper", "StringLiteralTypo")]
    [InlineData("a", "a", "A")]
    [InlineData("b", "b", "B")]
    [InlineData("m", "m", "M")]
    [InlineData("z", "z", "Z")]
    [InlineData("A", "a", "A")]
    [InlineData("B", "b", "B")]
    [InlineData("M", "m", "M")]
    [InlineData("Z", "z", "Z")]
    [InlineData("a0", "a0", "A0")]
    [InlineData("aa", "aa", "aA", "Aa", "AA")]
    [InlineData("aaa", "aaa", "aAa", "AaA", "AAA")]
    [InlineData("aaaa", "aaaa", "aAaA", "AaAa", "AAAA")]

    [InlineData("A0", "a0", "A0")]
    [InlineData("AA", "aa", "aA", "Aa", "AA")]
    [InlineData("AAA", "aaa", "aAa", "AaA", "AAA")]
    [InlineData("AAAA", "aaaa", "aAaA", "AaAa", "AAAA")]

    [InlineData("abcdefg", "abcdefg", "aBcDeFg", "AbCdEfG", "ABCDEFG")]
    [InlineData("test", "test", "tEsT", "TeSt", "TEST")]
    public void generates_xunit_test_cases_with_strings_of_lower_mixed_and_upper_casing(
        string input, params string[] expected)
    {
        AttributeCasesFrom(input).Should()
            .SatisfyRespectively(TestCases(expected));
    }

    [Theory, SuppressMessage("ReSharper", "StringLiteralTypo")]
    [InlineData("AbcDef", "AbcDef", "abcdef", "aBcDeF", "AbCdEf", "ABCDEF")]
    public void generates_xunit_test_cases_including_the_original_casing_if_not_contained_in_generated_ones(
        string input, params string[] expected)
    {
        AttributeCasesFrom(input).Should()
            .SatisfyRespectively(TestCases(expected));
    }

    private static IEnumerable<Action<object[]>> TestCases(IEnumerable<string> cases) =>
        cases.Select(TestCase);

    private static Action<object[]> TestCase(string arg) =>
        o => o.Should().Equal(arg);
}
