using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests;

public class The_case_variants_attribute
{
    private static IEnumerable<object[]> AttributeCasesFrom(string source)
    {
        return new CaseVariantDataAttribute(source).GetData(default!);
    }

    [Fact]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Generates_variants_of_original_string_with_upper_lower_and_mixed_casing()
    {
        ((int)'a').Should().Be(97);
        ((int)'A').Should().Be(65);
        ((char)('z' + 1)).Should().Be('{');
        ((char)('Z' + 1)).Should().Be('[');
        ((char)('a' - 1)).Should().Be('`');
        ((char)('A' - 1)).Should().Be('@');
        ('a'-'A').Should().Be(32);


        "".CaseVariants().Should().Equal("");
        " ".CaseVariants().Should().Equal(" ");
        "  ".CaseVariants().Should().Equal("  ");

        "a".CaseVariants().Should().Equal("a", "A");
        "b".CaseVariants().Should().Equal("b", "B");
        "c".CaseVariants().Should().Equal("c", "C");
        "m".CaseVariants().Should().Equal("m", "M");
        "z".CaseVariants().Should().Equal("z", "Z");

        "A".CaseVariants().Should().Equal("a", "A");
        "B".CaseVariants().Should().Equal("b", "B");
        "C".CaseVariants().Should().Equal("c", "C");
        "M".CaseVariants().Should().Equal("m", "M");
        "Z".CaseVariants().Should().Equal("z", "Z");

        "0".CaseVariants().Should().Equal("0");
        "{".CaseVariants().Should().Equal("{");
        "[".CaseVariants().Should().Equal("[");
        "`".CaseVariants().Should().Equal("`");
        "@".CaseVariants().Should().Equal("@");

        "a0".CaseVariants().Should().Equal("a0", "A0");
        "aa".CaseVariants().Should().Equal("aa", "aA", "Aa", "AA");
        "aaa".CaseVariants().Should().Equal("aaa", "aAa", "AaA", "AAA");
        "aaaa".CaseVariants().Should().Equal("aaaa", "aAaA", "AaAa", "AAAA");

        "A0".CaseVariants().Should().Equal("a0", "A0");
        "AA".CaseVariants().Should().Equal("aa", "aA", "Aa", "AA");
        "AAA".CaseVariants().Should().Equal("aaa", "aAa", "AaA", "AAA");
        "AAAA".CaseVariants().Should().Equal("aaaa", "aAaA", "AaAa", "AAAA");

        "abcdefghijklmnopqrstuvwxyz".CaseVariants().Should().Equal(
            "abcdefghijklmnopqrstuvwxyz",
            "aBcDeFgHiJkLmNoPqRsTuVwXyZ",
            "AbCdEfGhIjKlMnOpQrStUvWxYz",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
    }

    [Fact]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void Generates_xunit_cases_of_original_string_with_upper_lower_and_mixed_casing()
    {
        AttributeCasesFrom("test").Should().SatisfyRespectively(
            o => o.Should().ContainSingle().Which.Should().Be("test"),
            o => o.Should().ContainSingle().Which.Should().Be("tEsT"),
            o => o.Should().ContainSingle().Which.Should().Be("TeSt"),
            o => o.Should().ContainSingle().Which.Should().Be("TEST")
            );
    }
}
