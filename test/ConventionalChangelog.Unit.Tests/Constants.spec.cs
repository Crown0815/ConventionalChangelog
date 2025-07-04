using AwesomeAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests;

public class The_constant
{
    public static readonly TheoryData<string> BreakingChangeFooterTokens =
    [
        "BREAKING CHANGE",
        "BREAKING-CHANGE",
    ];

    [Fact]
    public void output_changelog_has_the_expected_value()
    {
        Output.Changelog.Should().Be("CRN.Changelog");
    }
}
