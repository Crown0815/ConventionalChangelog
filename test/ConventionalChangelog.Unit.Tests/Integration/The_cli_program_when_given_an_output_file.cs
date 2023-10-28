using System;
using System.IO;
using FluentAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Integration;

public class The_cli_program_when_given_an_output_file : CliTestsBase
{
    private const string OutputKeyShort = "-o";
    private const string OutputKeyLong = "--output";
    private readonly string _fileName = Guid.NewGuid().ToString();

    [Theory]
    [InlineData(OutputKeyShort)]
    [InlineData(OutputKeyLong)]
    public void prints_changelog_into_the_file_given_through_the(string argument)
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput($"{argument} {_fileName} {Repository.Path()}");

        output.Should().BeEmpty();
        File.ReadAllText(_fileName).Should().Be(A.Changelog.WithGroup(Feature, 1) + Environment.NewLine);
    }

    [Theory]
    [InlineData(OutputKeyShort)]
    [InlineData(OutputKeyLong)]
    public void run_from_teamcity_prints_a_service_message_setting_a_parameter_to_the_changelog(string argument)
    {
        Repository.Commit(Feature, 1);

        var output = OutputWithInput($"{argument} {_fileName} {Repository.Path()}", (TeamCity.EnvironmentVariable, "whatever"));

        output.Should().Be(TeamCity.SetParameterCommand("CRN.Changelog", A.Changelog.WithGroup(Feature, 1)) + NewLine);
    }

    public override void Dispose()
    {
        File.Delete(_fileName);
        base.Dispose();
    }
}
