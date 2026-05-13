using ConventionalChangelog.BuildSystems;
using AwesomeAssertions;
using Xunit;
using static System.Environment;

namespace ConventionalChangelog.Unit.Tests.BuildSystem;

public class GitHub_interaction_specs
{
    // based on https://docs.github.com/en/actions/writing-workflows/choosing-what-your-workflow-does/workflow-commands-for-github-actions#setting-an-output-parameter

    private static string OutputCommandToSetParameter(string name, string value) =>
        $"{name}<<EOF{NewLine}{value}{NewLine}EOF";

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SomeValue")]
    [InlineData("\u007f")]
    [InlineData("~")]
    [InlineData("🚀")]
    [InlineData("🧑‍🔬")]
    public void GitHub_set_output_command_for_a_given_parameter_name_to_a_value_returns_output_command_format(string content)
    {
        var message = GitHub.GenerateContent("Parameter.Name", content);
        message.Should().Be(OutputCommandToSetParameter("Parameter.Name", content));
    }
}
