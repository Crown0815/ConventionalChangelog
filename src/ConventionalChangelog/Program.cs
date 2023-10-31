using ConventionalChangelog;

var output = (string?)null;
var repositoryPath = args[0];

if (args[0] is "-o" or "--output")
{
    output = args[1];
    repositoryPath = args[2];
}

var changelog = new Changelog(Configured.Default()).FromRepository(repositoryPath);

if (output is not null)
{
    File.WriteAllText(output, changelog + Environment.NewLine);
    if (Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null)
        Console.WriteLine(TeamCity.SetParameterCommand("CRN.Changelog", changelog));
}
else
{
    if (Environment.GetEnvironmentVariable(TeamCity.EnvironmentVariable) is not null)
        Console.WriteLine(TeamCity.SetParameterCommand("CRN.Changelog", changelog));
    else
        Console.WriteLine(changelog);
}
