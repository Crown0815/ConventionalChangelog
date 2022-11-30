using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests.Changelog_specs;

public class Conventional_commit_parsing_specs
{
    // Conventional commit specification:
    // https://www.conventionalcommits.org/en/v1.0.0/#specification

    private static class ConventionalCommit
    {
        public const string Type = "type";
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

    private readonly ConventionalCommitMessage _parsed;

    public Conventional_commit_parsing_specs()
    {
        _parsed = ConventionalCommitMessage.Parse(ConventionalCommit.Message);
    }

    [Fact]
    public void The_parser_extracts_the_type_indicator_from_a_conventional_commit()
    {
        _parsed.Type.Should().Be(ConventionalCommit.Type);
    }

    [Fact]
    public void The_parser_extracts_the_description_from_a_conventional_commit()
    {
        _parsed.Description.Should().Be(ConventionalCommit.Description);
    }

    [Fact]
    public void The_parser_extracts_the_body_from_a_conventional_commit()
    {
        _parsed.Body.Should().Be(ConventionalCommit.Body);
    }

    [Fact]
    public void The_parser_extracts_the_footer_from_a_conventional_commit()
    {
        _parsed.Footers.Should().BeEquivalentTo(new ConventionalCommitFooter[]
        {
            new(ConventionalCommit.FooterToken, ConventionalCommit.FooterValue),
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
    public void The_parser_extracts_the_specific(string footer, string expectedToken, string expectedValue)
    {
        var messageWithSpecificFooter = ConventionalCommit.Message.Replace(ConventionalCommit.Footer, footer);
        var parsed = ConventionalCommitMessage.Parse(messageWithSpecificFooter);

        parsed.Footers.Should().BeEquivalentTo(new ConventionalCommitFooter[]
        {
            new(expectedToken, expectedValue),
        });
    }

    [Fact]
    public void The_parser_extracts_multiple_footers()
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
        var parsed = ConventionalCommitMessage.Parse(messageWithSpecificFooter);

        parsed.Footers.Should().BeEquivalentTo(new ConventionalCommitFooter[]
        {
            new("token1", "value1"),
            new("token-2", "value 2"),
            new("token3", "value 3"),
            new("token-4", $"value{Environment.NewLine}with extra line"),
            new("token-5", $"value{Environment.NewLine}{Environment.NewLine}with blank line"),
        });
    }
}
