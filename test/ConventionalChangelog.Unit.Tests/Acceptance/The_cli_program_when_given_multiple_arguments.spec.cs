using System;
using System.IO;
using AwesomeAssertions;
using Xunit;
using static System.Environment;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public sealed class The_cli_program_when_given_multiple_arguments : CliTestsBase
{
    private readonly string _fileName = Guid.NewGuid().ToString();

    [Fact]
    public void handles_each_of_them_individually()
    {
        Repository.Commit(Feature, 0).Tag("r1.0.0");
        Repository.Commit(Feature, 1).Tag("r1.0.0-alpha");
        Repository.Commit(Feature, 2);

        var output = OutputWithInput($"-t r -o {_fileName} -i {Repository.Path()}");

        output.Should().BeEmpty();
        File.ReadAllText(_fileName).Should().Be(A.Changelog.WithGroup(Feature, 2, 1) + NewLine);
    }

    public override void Dispose()
    {
        File.Delete(_fileName);
        base.Dispose();
    }
}
