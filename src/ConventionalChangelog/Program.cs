using ConventionalChangelog;

var changelog = Changelog.FromRepository(args[0], Configuration.Default());

if (Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null)
    changelog = TeamCity.SetParameterCommand("CRN.Changelog", changelog);

Console.WriteLine(changelog);
