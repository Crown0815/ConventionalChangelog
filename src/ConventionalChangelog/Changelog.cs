using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public static class Changelog
{
    public static string FromRepository(string path, IConfiguration configuration)
    {
        var commits = RepositoryReader.CommitsFrom(path, configuration);
        return From(commits, configuration);
    }

    public static string From(IEnumerable<Commit> messages, IConfiguration configuration)
    {
        var parser = new MessageParser2(configuration);
        var logEntries = messages.Select(commit => parser.Parse(commit.Message) with { Hash = commit.Hash })
            .Reduce()
            .SelectMany(AsPrintable);

        return configuration.Ordered(logEntries)
            .Aggregate(new LogAggregate(configuration), Add)
            .ToString();
    }

    private static IEnumerable<IPrintable> AsPrintable(CommitMessage message)
    {
        return message
            .Footers.OfType<IPrintable>()
            .Prepend(message);
    }

    private static LogAggregate Add(LogAggregate a, IPrintable p) => a.Add(p.TypeIndicator, p.Description);
}
