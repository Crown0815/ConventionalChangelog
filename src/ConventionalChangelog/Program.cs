using ConventionalChangelog;

var changelog = Changelog.FromRepository(args[0], new Configuration());

if (Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null)
    changelog = TeamCity.SetParameterCommand("CRN.Changelog", changelog);

Console.WriteLine(changelog);
