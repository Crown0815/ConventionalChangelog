using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Changelog
{
    private readonly RepositoryReader _repositoryReader;
    private readonly MessageParser _parser;
    private readonly RelationshipResolver _relationshipResolver;
    private readonly LogWriter _logWriter;

    public Changelog(Configuration configuration)
    {
        IConfigured configured = new Configured(configuration);
        _relationshipResolver = new RelationshipResolver(configuration);
        _repositoryReader = new RepositoryReader(configured);
        _parser = new MessageParser(configured);
        _logWriter = new LogWriter(configured);
    }

    public string FromRepository(string path)
    {
        var commits = _repositoryReader.CommitsFrom(path);
        return From(commits);
    }

    public string From(IEnumerable<Commit> commits)
    {
        var messages = Parse(commits);
        return _logWriter.Print(PrintReady(messages));
    }

    private IEnumerable<CommitMessage> Parse(IEnumerable<Commit> commits)
    {
        return commits.Select(_parser.Parse);
    }

    private IEnumerable<IPrintable> PrintReady(IEnumerable<CommitMessage> messages)
    {
        return _relationshipResolver
            .ResolveRelationshipsBetween(messages)
            .SelectMany(AsPrintable);
    }

    private static IEnumerable<IPrintable> AsPrintable(CommitMessage message)
    {
        return message.Footers.OfType<IPrintable>().Prepend(message);
    }
}
