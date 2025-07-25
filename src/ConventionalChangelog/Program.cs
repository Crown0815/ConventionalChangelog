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
    [Option('r')]bool skipTitle,
    [Option('a')]bool showHash,
    [Option('c')]ChangelogOrder? changelogOrder,
    [Option('x')]string? referenceCommit,
    [Argument]string repositoryPath)
{
    var configuration = new Configuration(
        ignorePrerelease: ignorePrereleases,
        versionTagPrefix: tagPrefix,
        skipTitle: skipTitle,
        ignoreScope: ignoreScope,
        referenceCommit: referenceCommit,
        changelogOrder: changelogOrder,
        showHash: showHash
        );

    var changelog = new Changelog(configuration).FromRepository(repositoryPath);

    if (output is not null)
    {
        File.WriteAllText(output, changelog + Environment.NewLine);
        if (TeamCity.IsCurrentCi())
            Console.WriteLine(TeamCity.SetParameterCommand(Output.Changelog, changelog));
    }
    else
    {
        Console.WriteLine(TeamCity.IsCurrentCi()
            ? TeamCity.SetParameterCommand(Output.Changelog, changelog)
            : changelog);
    }
}
