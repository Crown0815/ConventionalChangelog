using ConventionalReleaseNotes;

var changelog = Changelog.FromRepository(args[0]);

if (Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null)
    changelog = TeamCity.SetParameterCommand("CRN.Changelog", changelog);

Console.WriteLine(changelog);
