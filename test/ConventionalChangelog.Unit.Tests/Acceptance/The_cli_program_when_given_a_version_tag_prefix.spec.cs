using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_version_tag_prefix : CliTestsBase
{
    private const string SemanticVersion = "1.0.0";

    [Theory, CombinatorialData]
    public void literal_prints_changelog_from_last_commit_with_matching_version_tag(
        [CombinatorialMemberData(nameof(TagPrefixKeys))] string argument,
        [CombinatorialValues('t', 'r', "ver")] string prefix
    )
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag($"{prefix}{SemanticVersion}");
        Repository.Commit(Feature, 1);

        var output = OutputWithInput($"{argument} {prefix} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }

    [Theory, CombinatorialData]
    public void regular_expression_prints_changelog_from_last_commit_with_matching_version_tag(
        [CombinatorialMemberData(nameof(TagPrefixKeys))] string argument,
        [CombinatorialValues('t', 'r', "ver")] string prefix
    )
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag($"{prefix}{SemanticVersion}");
        Repository.Commit(Feature, 1);

        var output = OutputWithInput($"{argument} [tr]|ver {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }

    [Theory, CombinatorialData]
    public void prints_changelog_while_ignoring_non_matching_version_tags(
        [CombinatorialMemberData(nameof(TagPrefixKeys))] string argument,
        [CombinatorialValues('t', 'r', "ver", "[tr]")] string pattern
    )
    {
        Repository.Commit(Feature, -1);
        Repository.Commit(Feature, 0).Tag($"v{SemanticVersion}");
        Repository.Commit(Feature, 1);

        var output = OutputWithInput($"{argument} {pattern} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 1, 0, -1) + NewLine);
    }
}
