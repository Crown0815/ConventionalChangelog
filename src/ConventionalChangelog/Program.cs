﻿using Cocona;
using ConventionalChangelog;
using ConventionalChangelog.BuildSystems;

CoconaLiteApp.Run(Execute);
return;

void Execute(
    [Option('o')]string? output,
    [Option('t')]string? tagPrefix,
    [Option('i')]bool ignorePrereleases,
    [Option('s')]bool ignoreScope,
    [Argument]string repositoryPath)
{
    var configuration = new Configuration(ignoreScope: ignoreScope)
    {
        ChangelogOrder = default,
        VersionTagPrefix = tagPrefix!,
        IgnorePrerelease = ignorePrereleases,
    };
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
}

bool IsRunningInTeamCityCi()
{
    return Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null;
}

string TeamCityParameterMessageFrom(string s)
{
    return TeamCity.SetParameterCommand("CRN.Changelog", s);
}
