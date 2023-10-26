using System;
using ConventionalChangelog.Conventional;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

public class Parsing_a_commit_message_from
{
    private readonly ITypeFinder _configuration = Configuration.Default();

    [Fact]
    public void an_empty_string_returns_an_empty_commit_message()
    {
        var parsed = CommitMessage.Parse("", _configuration);
        parsed.Type.Should().Be(new CommitType("", "", Relevance.Ignore));
        parsed.Body.Should().BeEmpty();
        parsed.Description.Should().BeEmpty();
        parsed.Footers.Should().BeEmpty();
    }

    [Fact]
    public void a_null_string_throws_an_argument_null_exception()
    {
        var parsingNull = () => CommitMessage.Parse(null!, _configuration);
        parsingNull.Should().Throw<ArgumentNullException>();
    }
}
