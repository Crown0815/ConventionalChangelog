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
    [Option('a')]bool showHash,
    [Option('c')]ChangelogOrder? changelogOrder,
    [Option('x')]string? referenceCommit,
    [Option('f')]string? configFile,
    [Argument]string repositoryPath)
{
    var fileConfiguration = configFile is null
        ? default
        : ConfigurationFile.Read(configFile);

    var configuration = new Configuration(
        ignorePrerelease: ignorePrereleases,
        versionTagPrefix: tagPrefix,
        skipTitle: skipTitle,
        commitTypes: fileConfiguration.CommitTypes,
        scopes: fileConfiguration.Scopes,
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
        {
            TeamCity.WriteOutput(Output.ChangelogLegacy, changelog);
            TeamCity.WriteOutput(Output.Changelog, changelog);
        }
        else if (GitHub.IsCurrentCi())
        {
            GitHub.WriteOutput(Output.ChangelogLegacy, changelog);
            GitHub.WriteOutput(Output.Changelog, changelog);
        }
    }
    else
    {
        if (TeamCity.IsCurrentCi())
        {
            TeamCity.WriteOutput(Output.ChangelogLegacy, changelog);
            TeamCity.WriteOutput(Output.Changelog, changelog);
        }
        else if (GitHub.IsCurrentCi())
        {
            GitHub.WriteOutput(Output.ChangelogLegacy, changelog);
            GitHub.WriteOutput(Output.Changelog, changelog);
        }
        else
            Console.WriteLine(changelog);
    }
}
