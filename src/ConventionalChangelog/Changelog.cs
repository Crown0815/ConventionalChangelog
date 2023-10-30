using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Changelog
{
    private readonly IConfiguration _configuration;
    private readonly RepositoryReader _repositoryReader;
    private readonly MessageParser _parser;

    public Changelog(IConfiguration configuration)
    {
        _configuration = configuration;
        _repositoryReader = new RepositoryReader(_configuration);
        _parser = new MessageParser(_configuration);
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

        return _configuration.Ordered(logEntries)
            .Aggregate(new LogAggregate(_configuration), Add)
            .ToString();
    }

    private static IEnumerable<IPrintable> AsPrintable(CommitMessage message)
    {
        return message.Footers.OfType<IPrintable>().Prepend(message);
    }

    private static LogAggregate Add(LogAggregate a, IPrintable p) => a.Add(p.TypeIndicator, p.Description);
}
