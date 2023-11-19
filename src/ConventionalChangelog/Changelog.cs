using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Changelog
{
    private readonly IConfigured _configured;
    private readonly RepositoryReader _repositoryReader;
    private readonly MessageParser _parser;
    private readonly MessageLinq.Strategy[] _strategies;

    public Changelog(Configuration configuration)
    {
        _configured = new Configured(configuration);
        _repositoryReader = new RepositoryReader(_configured);
        _parser = new MessageParser(_configured);

        _strategies = new MessageLinq.Strategy[] {
            new(configuration.DropSelf, true, true, true),
            new(configuration.DropBoth, false, false, true),
            new(configuration.DropOther, false, true, false),
        };
    }

    public string FromRepository(string path)
    {
        return From(_repositoryReader.CommitsFrom(path));
    }

    public string From(IEnumerable<Commit> messages)
    {
        var logEntries = messages.Select(commit => _parser.Parse(commit.Message) with { Hash = commit.Hash })
            .Reduce(_strategies)
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
