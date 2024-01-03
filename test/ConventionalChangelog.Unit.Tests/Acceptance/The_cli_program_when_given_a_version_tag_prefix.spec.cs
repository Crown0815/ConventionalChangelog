using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_version_tag_prefix : CliTestsBase
{
    private const string TagPrefixKeyShort = "-t";
    private const string TagPrefixKeyLong = "--tag-prefix";

    public static TheoryData<string> OutputKeys => new() { TagPrefixKeyShort, TagPrefixKeyLong };

    [Theory]
    [MemberData(nameof(OutputKeys))]
    public void prints_changelog_into_the_file_given_through_the(string argument)
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag("p1.0.0-alpha.1");
        Repository.Commit(Feature, 1);

        var output = OutputWithInput($"{argument} [p] {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }
}
