using System;
using System.IO;
using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_a_configuration_file : CliTestsBase
{
    public static TheoryData<string> ConfigurationFileKeysData { get; } = TheoryDataFrom(ConfigurationFile);

    [Theory, MemberData(nameof(ConfigurationFileKeysData))]
    public void uses_configured_commit_types(string argument)
    {
        Repository.Commit(Bugfix, A.Description(1));
        var configFile = CreateConfigFile(
            """
            CommitTypes:
              - TypeIndicator: fix
                GroupHeader: Corrective Actions
            """);

        var output = OutputWithInput($"{argument} {configFile} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(new CommitType("fix", "Corrective Actions", Relevance.Show), 1) + NewLine);
    }

    [Theory, MemberData(nameof(ConfigurationFileKeysData))]
    public void uses_configured_scopes(string argument)
    {
        Repository.Commit(Feature, "infra", A.Description(1));
        var configFile = CreateConfigFile(
            """
            Scopes:
              - Name: infra
                Header: Infrastructure
            """);

        var output = OutputWithInput($"{argument} {configFile} {Repository.Path()}");

        output.Should().Be(A.Changelog.WithGroup(Feature, "Infrastructure", 1) + NewLine);
    }

    private static string CreateConfigFile(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.yaml");
        File.WriteAllText(path, content);
        return path;
    }
}
