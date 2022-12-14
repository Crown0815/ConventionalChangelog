using System;
using System.Collections.Generic;
using ConventionalReleaseNotes.Conventional;
using FluentAssertions;
using Xunit;
using static ConventionalReleaseNotes.Conventional.CommitMessage;

namespace ConventionalReleaseNotes.Unit.Tests.Changelog_specs;

public class Parsing_a_conventional_commit_message
{
    // Conventional commit specification:
    // https://www.conventionalcommits.org/en/v1.0.0/#specification

    private static class ConventionalCommit
    {
        public const string Type = "feat";
        public const string Description = "description";

        public const string Body = """
                                   Body with
                                   newlines 	 tabs

                                      and some spaces


                                   and multiple blank lines
                                   """;

        public const string FooterToken = "token";
        public const string FooterValue = "value";
        public const string Footer = $"{FooterToken}: {FooterValue}";

        public const string Message = $"""
            {Type}: {Description}

            {Body}

            {Footer}
            """ ;
    }

    private readonly CommitMessage _parsed;

    public Parsing_a_conventional_commit_message()
    {
        _parsed = Parse(ConventionalCommit.Message);
    }

    [Fact]
    public void extracts_its_type_indicator()
    {
        _parsed.Type.Indicator.Should().Be(ConventionalCommit.Type);
    }

    [Fact]
    public void extracts_its_description()
    {
        _parsed.Description.Should().Be(ConventionalCommit.Description);
    }

    [Fact]
    public void extracts_its_body()
    {
        _parsed.Body.Should().Be(ConventionalCommit.Body);
    }

    [Fact]
    public void with_a_single_footer_extracts_its_footer()
    {
        _parsed.Footers.Should().BeEquivalentTo(new Footer[]
        {
            new(ConventionalCommit.FooterToken, ConventionalCommit.FooterValue),
        });
    }

    [Fact]
    public void with_multiple_footers_extracts_all_footers()
    {
        const string footers = """
                      token1: value1
                      token-2: value 2

                      token3 #value 3
                      token-4 #value
                      with extra line

                      token-5: value

                      with blank line
                      """;

        var messageWithSpecificFooter = ConventionalCommit.Message.Replace(ConventionalCommit.Footer, footers);
        var parsed = Parse(messageWithSpecificFooter);

        parsed.Footers.Should().BeEquivalentTo(new Footer[]
        {
            new("token1", "value1"),
            new("token-2", "value 2"),
            new("token3", "value 3"),
            new("token-4", $"value{Environment.NewLine}with extra line"),
            new("token-5", $"value{Environment.NewLine}{Environment.NewLine}with blank line"),
        });
    }

    private static readonly string[] Tokens =
    {
        "token",
        "token-with-dash",
        "BREAKING CHANGE",
        "BREAKING-CHANGE",
    };

    private static readonly string[] Separators = { ": ", " #" };

    private static readonly string[] Values =
    {
        "value",
        "value with spaces",
        "value	with	tabs",
        """
        value with
        linebreak
        """,
        """
        value with

        blank line
        """,
    };

    public static IEnumerable<object[]> GitTrailerConventionFooters()
    {
        foreach (var token in Tokens)
        foreach (var separator in Separators)
        foreach (var value in Values)
            yield return new object[] { token + separator + value, token, value };
    }

    public static IEnumerable<object[]> YouTrackConventionFooters()
    {
        yield return new object[] { "#SWX-1234", "SWX-1234", "" };
        yield return new object[] { "#SWX-1234 command", "SWX-1234", "command" };
        yield return new object[] { "#SWX-1234 command1 command2", "SWX-1234", "command1 command2" };
    }

    [Theory]
    [MemberData(nameof(GitTrailerConventionFooters))]
    [MemberData(nameof(YouTrackConventionFooters))]
    public void extracts_from_a(string formattedFooter, string theToken, string andTheValue)
    {
        var messageWithSpecificFooter = ConventionalCommit.Message.Replace(ConventionalCommit.Footer, formattedFooter);
        var parsed = Parse(messageWithSpecificFooter);

        parsed.Footers.Should().BeEquivalentTo(new Footer[] { new(theToken, andTheValue) });
    }
}
