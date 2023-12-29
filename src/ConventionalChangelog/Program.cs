using ConventionalChangelog;

var output = (string?)null;
var repositoryPath = args[0];

if (args[0] is "-o" or "--output")
{
    output = args[1];
    repositoryPath = args[2];
}

var configuration = new Configuration { ChangelogOrder = default};
var changelog = new Changelog(configuration).FromRepository(repositoryPath);

if (output is not null)
{
    File.WriteAllText(output, changelog + Environment.NewLine);
    if (IsRunningInTeamCityCi())
        Console.WriteLine(TeamCityParameterMessageFrom(changelog));
}
else
{
    Console.WriteLine(IsRunningInTeamCityCi()
        ? TeamCityParameterMessageFrom(changelog)
        : changelog);
}

return;

bool IsRunningInTeamCityCi()
{
    return Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null;
}

string TeamCityParameterMessageFrom(string s)
{
    return TeamCity.SetParameterCommand("CRN.Changelog", s);
}
