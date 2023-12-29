using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Changelog
{
    private readonly RepositoryReader _repositoryReader;
    private readonly MessageParser _parser;
    private readonly RelationshipResolver _relationshipResolver;
    private readonly LogWriter _logWriter;

    public Changelog(IConfiguration configuration)
    {
        var customization = new Customization(configuration);
        _repositoryReader = new RepositoryReader(customization);
        _parser = new MessageParser(customization);
        _relationshipResolver = new RelationshipResolver(customization);
        _logWriter = new LogWriter(customization);
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
