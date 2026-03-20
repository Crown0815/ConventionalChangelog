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

    [Fact]
    public void GitHub_set_output_command_for_a_given_parameter_name_to_a_value_returns_output_command_format()
    {
        var message = GitHub.SetOutputCommand("Parameter.Name", "NewValue");
        message.Should().Be(OutputCommandToSetParameter("Parameter.Name", "NewValue"));
    }

    [Theory]
    // non ASCII characters represented as unicode
    [InlineData("\u03a0", "\\u03a0")]
    [InlineData("\u0080", "\\u0080")]

    // ASCII characters not escaped
    [InlineData("\u007f", "\u007f")]
    [InlineData("\u007e", "~")]

    // All together
    [InlineData(
        "This 'string' [text] with \n\r |\u00b0| and |\u03a0|",
        "This 'string' [text] with \n\r |\\u00b0| and |\\u03a0|")]
    public void GitHub_set_output_command_escapes(string raw, string with)
    {
        var message = GitHub.SetOutputCommand("Parameter.Name", raw);
        message.Should().Be(OutputCommandToSetParameter("Parameter.Name", with));
    }
}
