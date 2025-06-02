using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_ignore_prereleases_flag : CliTestsBase
{
    private const string SemanticVersion = "1.0.0";
    private const string SemanticPrereleaseVersion = "1.0.0-alpha";
    public static TheoryData<string> IgnorePrereleaseKeysData { get; } = TheoryDataFrom(IgnorePrerelease);

    [Theory, MemberData(nameof(IgnorePrereleaseKeysData))]
    public void literal_prints_changelog_from_last_commit_with_matching_version_tag(string argument)
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag($"v{SemanticVersion}");
        Repository.Commit(Feature, 1).Tag($"v{SemanticPrereleaseVersion}");
        Repository.Commit(Feature, 2);

        var output = OutputWithInput($"{argument} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 2, 1) + NewLine);
    }
}
