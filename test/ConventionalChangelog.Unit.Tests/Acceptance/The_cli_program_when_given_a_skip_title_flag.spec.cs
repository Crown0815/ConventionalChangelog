using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_skip_title_flag : CliTestsBase
{
    public static TheoryData<string> SkipTitleFlagData { get; } = TheoryDataFrom(SkipTitleFlag);

    [Theory, MemberData(nameof(SkipTitleFlagData))]
    public void prints_changelog_without_title(string argument)
    {
        Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2);

        var output = OutputWithInput($"{argument} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithoutTitle().And(Feature, 2, 1) + NewLine);
    }
}
