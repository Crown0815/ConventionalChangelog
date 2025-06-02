using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_ignore_scopes_flag : CliTestsBase
{
    public static TheoryData<string> IgnoreScopeKeysData { get; } = TheoryDataFrom(IgnoreScope);

    [Theory, MemberData(nameof(IgnoreScopeKeysData))]
    public void is_set_excludes_the_scope_from_the_output(string argument)
    {
        Repository.Commit(Feature, "scope", A.Description(1));

        var output = OutputWithInput($"{argument} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, 1) + NewLine);
    }

    [Fact]
    public void is_unset_includes_the_scope_in_the_output()
    {
        Repository.Commit(Feature, "scope", A.Description(1));

        var output = OutputWithInput($"{Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, "scope", 1) + NewLine);
    }
}
