using Cocona;
using ConventionalChangelog;
using ConventionalChangelog.BuildSystems;

CoconaLiteApp.Run(Execute);
return;

void Execute(
    [Option('o')]string? output,
    [Option('t')]string? tagPrefix,
    [Option('i')]bool ignorePrereleases,
    [Option('s')]bool ignoreScope,
    [Option('r')]bool skipTitle,
    [Argument]string repositoryPath)
{
    var configuration = new Configuration(skipTitle: skipTitle, ignoreScope: ignoreScope)
    {
        ChangelogOrder = default,
        VersionTagPrefix = tagPrefix!,
        IgnorePrerelease = ignorePrereleases,
    };
    var changelog = new Changelog(configuration).FromRepository(repositoryPath);

    if (output is not null)
    {
        File.WriteAllText(output, changelog + Environment.NewLine);
        if (TeamCity.IsCurrentCi())
            Console.WriteLine(TeamCityParameterMessageFrom(changelog));
    }
    else
    {
        Console.WriteLine(TeamCity.IsCurrentCi()
            ? TeamCityParameterMessageFrom(changelog)
            : changelog);
    }
}

string TeamCityParameterMessageFrom(string s)
{
    return TeamCity.SetParameterCommand("CRN.Changelog", s);
}
