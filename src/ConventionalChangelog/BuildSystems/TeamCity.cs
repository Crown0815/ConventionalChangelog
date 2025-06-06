﻿using System.Text;

namespace ConventionalChangelog.BuildSystems;

public static class TeamCity
{
    // see https://www.jetbrains.com/help/teamcity/predefined-build-parameters.html#Predefined+Server+Build+Parameters
    // [...] This property can be used to determine if the build is run within TeamCity.
    public const string EnvironmentVariable = "TEAMCITY_VERSION";

    // see https://www.jetbrains.com/help/teamcity/service-messages.html for details
    public static string SetParameterCommand(string name, string value)
    {
        return $"##teamcity[setParameter name='{name}' value='{Escaped(value)}']";
    }

    public static bool IsCurrentCi()
    {
        return Environment.GetEnvironmentVariable(EnvironmentVariable) is not null;
    }

    private static string Escaped(string raw) => raw
        .Replace("|", "||")
        .Replace("'", "|'")
        .Replace("[", "|[")
        .Replace("]", "|]")
        .Replace("\n", "|n")
        .Replace("\r", "|r")
        .ReplaceUniCode();

    private static string ReplaceUniCode(this string raw) =>
        raw.Aggregate(new StringBuilder(), Append).ToString();

    private static StringBuilder Append(StringBuilder b, char character) =>
        b.Append(Escaped(character));

    private static object Escaped(char c) => char.IsAscii(c)
        ? c
        : Escaped((int)c);

    private static string Escaped(int c) => "|0x" + c.ToString("x4");

}
