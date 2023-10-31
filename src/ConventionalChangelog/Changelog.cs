using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Changelog
{
    private readonly IConfigured _configured;
    private readonly RepositoryReader _repositoryReader;
    private readonly MessageParser _parser;

    public Changelog(Configuration configuration)
    {
        _configured = new Configured(configuration);
        _repositoryReader = new RepositoryReader(_configured);
        _parser = new MessageParser(_configured);
    }

    public string FromRepository(string path)
    {
        return From(_repositoryReader.CommitsFrom(path));
    }

    public string From(IEnumerable<Commit> messages)
    {
        var logEntries = messages.Select(commit => _parser.Parse(commit.Message) with { Hash = commit.Hash })
            .Reduce()
            .SelectMany(AsPrintable);

        return _configured.Ordered(logEntries)
            .Aggregate(new LogAggregate(_configured), Add)
            .ToString();
    }

    private static IEnumerable<IPrintable> AsPrintable(CommitMessage message)
    {
        return message.Footers.OfType<IPrintable>().Prepend(message);
    }

    private static LogAggregate Add(LogAggregate a, IPrintable p) => a.Add(p.TypeIndicator, p.Description);
}
