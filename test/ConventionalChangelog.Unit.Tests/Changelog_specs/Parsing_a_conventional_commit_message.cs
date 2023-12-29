using System;
using ConventionalChangelog.Conventional;
using FluentAssertions;
using Xunit;
using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class Parsing_a_conventional_commit_message
{
    private static readonly MessageParser MessageParser = new(new Configuration());
    private static CommitMessage Parsed(string message) => MessageParser.Parse(message);

    // Lb = Linebreak. The abbreviation was chosen to keep string definitions short
    private static readonly string Lb = Environment.NewLine;

    // Conventional commit specification:
    // https://www.conventionalcommits.org/en/v1.0.0/#specification

    private static class TestCommit
    {
        public const string Type = "feat";
        public const string Description = "description";

#if NET6_0
        public static readonly string Body = $"Body with{Lb}newlines \t tabs{Lb}{Lb}   and some spaces{Lb}{Lb}{Lb}and multiple blank lines";
        public static readonly string Message = $"{Type}: {Description}{Lb}{Lb}{Body}{Lb}{Lb}{Footer}" ;
#elif NET7_0_OR_GREATER
        public const string Body = """
            Body with
            newlines 	 tabs

               and some spaces


            and multiple blank lines
            """;

        public const string Message = $"""
            {Type}: {Description}

            {Body}

            {Footer}
            """ ;
#endif

        public const string FooterToken = "token";
        public const string FooterValue = "value";
        public const string Footer = $"{FooterToken}: {FooterValue}";
    }

    private readonly CommitMessage _parsed = Parsed(TestCommit.Message);

    [Fact]
    public void extracts_its_type_indicator()
    {
        _parsed.TypeIndicator.Should().Be(TestCommit.Type);
    }

    [Fact]
    public void extracts_its_description()
    {
        _parsed.Description.Should().Be(TestCommit.Description);
    }

    [Fact]
    public void extracts_its_body()
    {
        _parsed.Body.Should().Be(TestCommit.Body);
    }

    [Fact]
    public void with_a_single_footer_extracts_its_footer()
    {
        _parsed.Footers.Should().Equal(new Footer(TestCommit.FooterToken, TestCommit.FooterValue));
    }

    [Fact]
    public void with_multiple_footers_extracts_all_footers()
    {
        var footers = $"token1: value1{Lb}token-2: value 2{Lb}{Lb}token3 " +
                      $"#value 3{Lb}token-4 #value{Lb}with extra line{Lb}{Lb}" +
                      $"token-5: value{Lb}{Lb}with blank line";

        var message = TestCommit.Message.Replace(TestCommit.Footer, footers);
        var parsed = Parsed(message);

        parsed.Footers.Should().Equal(
            new Footer("token1", "value1"),
            new Footer("token-2", "value 2"),
            new Footer("token3", "value 3"),
            new Footer("token-4", $"value{Lb}with extra line"),
            new Footer("token-5", $"value{Lb}{Lb}with blank line"));
    }

    public static readonly string[] Separators = { ": ", " #" };

    public static readonly string[] Values =
    {
        "value",
        "value with spaces",
        "value	with	tabs",
        $"value with{Lb}linebreak",
        $"value with{Lb}{Lb}blank line",
    };

    public static readonly string[] Tokens =
    {
        "token",
        "token-with-dash",
        "BREAKING CHANGE",
        "BREAKING-CHANGE",
    };

    [Theory, CombinatorialData]
    public void extracts_the_parts_from_a_footer_formatted_using_a(
        [CombinatorialMemberData(nameof(Separators))] string separator,
        [CombinatorialMemberData(nameof(Tokens))] string aToken,
        [CombinatorialMemberData(nameof(Values))] string andAValue)
    {
        var formattedFooter = aToken + separator + andAValue;
        var message = TestCommit.Message.Replace(TestCommit.Footer, formattedFooter);
        var parsed = Parsed(message);

        parsed.Footers.Should().BeEquivalentTo(new Footer[]{new (aToken, andAValue)});
    }

    public static readonly string[] YouTrackValues =
    {
        "",
        "command",
        "command1 command2",
    };

    public static readonly string[] YouTrackTokens =
    {
        "SWX-1234",
        "DZE-12",
    };

    [Theory, CombinatorialData]
    public void extracts_the_parts_from_a_youtrack_footer_consisting_of(
        [CombinatorialMemberData(nameof(YouTrackTokens))] string aToken,
        [CombinatorialMemberData(nameof(YouTrackValues))] string andAValue)
    {
        var formattedFooter = "#" + aToken + " " + andAValue;
        var message = TestCommit.Message.Replace(TestCommit.Footer, formattedFooter);
        var parsed = Parsed(message);

        parsed.Footers.Should().BeEquivalentTo(new Footer[]{new (aToken, andAValue)});
    }
}
