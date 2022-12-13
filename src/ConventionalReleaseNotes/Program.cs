// See https://aka.ms/new-console-template for more information

using ConventionalReleaseNotes;

var changelog = Changelog.FromRepository(args[0]);

Console.WriteLine(changelog);
