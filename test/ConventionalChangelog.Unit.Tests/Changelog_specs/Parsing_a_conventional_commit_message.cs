using System;
using System.Collections.Generic;
using ConventionalChangelog.Conventional;
using FluentAssertions;
using Xunit;
using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class Parsing_a_conventional_commit_message
{
    private static readonly IConfiguration Config = Configuration.Default();

    // Lb = Linebreak. The abbreviation was chosen to keep string definitions short
    private static readonly string Lb = Environment.NewLine;

    // Conventional commit specification:
    // https://www.conventionalcommits.org/en/v1.0.0/#specification

    private static class ConventionalCommit
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

    private readonly CommitMessage _parsed = Parse(ConventionalCommit.Message, Config);

    [Fact]
    public void extracts_its_type_indicator()
    {
        _parsed.TypeIndicator.Should().Be(ConventionalCommit.Type);
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
        _parsed.Footers.Should().Equal(
            new Footer(ConventionalCommit.FooterToken, ConventionalCommit.FooterValue));
    }

    [Fact]
    public void with_multiple_footers_extracts_all_footers()
    {
        var footers = $"token1: value1{Lb}token-2: value 2{Lb}{Lb}token3 " +
                      $"#value 3{Lb}token-4 #value{Lb}with extra line{Lb}{Lb}" +
                      $"token-5: value{Lb}{Lb}with blank line";

        var messageWithSpecificFooter = ConventionalCommit.Message.Replace(ConventionalCommit.Footer, footers);
        var parsed = Parse(messageWithSpecificFooter, Config);

        parsed.Footers.Should().Equal(
            new Footer("token1", "value1"),
            new Footer("token-2", "value 2"),
            new Footer("token3", "value 3"),
            new Footer("token-4", $"value{Lb}with extra line"),
            new Footer("token-5", $"value{Lb}{Lb}with blank line"));
    }

    private static readonly string[] Separators = { ": ", " #" };

    private static readonly string[] Values =
    {
        "value",
        "value with spaces",
        "value	with	tabs",
        $"value with{Lb}linebreak",
        $"value with{Lb}{Lb}blank line",
    };

    public static IEnumerable<object[]> BreakingChangeConventionFooters()
    {
        foreach (var token in new[] { "BREAKING CHANGE", "BREAKING-CHANGE", })
        foreach (var separator in Separators)
        foreach (var value in Values)
            yield return new object[] { token + separator + value, token, value };
    }

    public static IEnumerable<object[]> GitTrailerConventionFooters()
    {
        foreach (var token in new[] { "token", "token-with-dash", })
        foreach (var separator in Separators)
        foreach (var value in Values)
            yield return new object[] { token + separator + value, token, value};
    }

    public static IEnumerable<object[]> YouTrackConventionFooters()
    {
        yield return new object[] { "#SWX-1234", "SWX-1234", "" };
        yield return new object[] { "#SWX-1234 command", "SWX-1234", "command" };
        yield return new object[] { "#SWX-1234 command1 command2", "SWX-1234", "command1 command2" };
    }

    [Theory]
    [MemberData(nameof(BreakingChangeConventionFooters))]
    [MemberData(nameof(GitTrailerConventionFooters))]
    [MemberData(nameof(YouTrackConventionFooters))]
    public void extracts_from_a(string formattedFooter, string theToken, string andTheValue)
    {
        var messageWithSpecificFooter = ConventionalCommit.Message.Replace(ConventionalCommit.Footer, formattedFooter);
        var parsed = Parse(messageWithSpecificFooter, Config);

        parsed.Footers.Should().BeEquivalentTo(new Footer[]{new (theToken, andTheValue)});
    }
}
