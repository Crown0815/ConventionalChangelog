using ConventionalChangelog.BuildSystems;
using FluentAssertions;
using Xunit;

namespace ConventionalChangelog.Unit.Tests.BuildSystem;

public class TeamCity_interaction_specs
{
    // based on https://www.jetbrains.com/help/teamcity/service-messages.html

    private static string ServiceMessageToSetParameter(string name, string value) =>
        $"##teamcity[setParameter name='{name}' value='{value}']";

    [Fact]
    public void TeamCity_set_parameter_command_for_a_given_parameter_name_to_a_value_returns_service_message_format()
    {
        var message = TeamCity.SetParameterCommand("Parameter.Name", "NewValue");
        message.Should().Be(ServiceMessageToSetParameter("Parameter.Name", "NewValue"));
    }

    [Theory]
    [InlineData("'", "|'")]
    [InlineData("\n", "|n")]
    [InlineData("\r", "|r")]
    [InlineData("|", "||")]
    [InlineData("[", "|[")]
    [InlineData("]", "|]")]

    // non ASCII characters represented as unicode
    [InlineData("\u03a0", "|0x03a0")]
    [InlineData("\u0080", "|0x0080")]

    // ASCII characters represented as unicode
    [InlineData("\u007f", "")]
    [InlineData("\u007e", "~")]

    // All together
    [InlineData(
        "This 'string' [text] with \n\r |\u00b0| and |\u03a0|",
        "This |'string|' |[text|] with |n|r |||0x00b0|| and |||0x03a0||")]
    public void TeamCity_set_parameter_command_escapes(string raw, string with)
    {
        var message = TeamCity.SetParameterCommand("Parameter.Name", raw);
        message.Should().Be(ServiceMessageToSetParameter("Parameter.Name", with));
    }
}
