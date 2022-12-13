using ConventionalReleaseNotes;

var changelog = Changelog.FromRepository(args[0]);
Console.WriteLine(changelog);
