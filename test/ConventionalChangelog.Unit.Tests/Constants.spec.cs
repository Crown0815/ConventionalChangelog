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
    public void legacy_output_changelog_has_the_expected_value()
    {
        Output.ChangelogLegacy.Should().Be("CRN.Changelog");
    }

    [Fact]
    public void output_changelog_has_the_expected_value()
    {
        Output.Changelog.Should().Be("Changelog");
    }
}
