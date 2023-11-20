using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public class Changelog
{
    private readonly IConfigured _configured;
    private readonly RepositoryReader _repositoryReader;
    private readonly MessageParser _parser;
    private readonly RelationshipResolver _relationshipResolver;

    public Changelog(Configuration configuration)
    {
        _configured = new Configured(configuration);
        _repositoryReader = new RepositoryReader(_configured);
        _parser = new MessageParser(_configured);
        _relationshipResolver = new RelationshipResolver(configuration);
    }

    public string FromRepository(string path)
    {
        return From(_repositoryReader.CommitsFrom(path));
    }

    public string From(IEnumerable<Commit> commits)
    {
        var commitMessages = commits.Select(_parser.Parse);
        var logMessages = _relationshipResolver.ResolveRelationshipsBetween(commitMessages).SelectMany(AsPrintable);
        return new LogWriter(_configured).Write(logMessages);
    }

    private static IEnumerable<IPrintable> AsPrintable(CommitMessage message)
    {
        return message.Footers.OfType<IPrintable>().Prepend(message);
    }
}
