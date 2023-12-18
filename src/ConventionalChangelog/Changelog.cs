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
        ICustomization customization = new Customization(configuration);
        _repositoryReader = new RepositoryReader(customization);
        _parser = new MessageParser(customization);
        _relationshipResolver = new RelationshipResolver(customization);
        _logWriter = new LogWriter(customization);
    }

    public string FromRepository(string path)
    {
        var rawCommits = _repositoryReader.CommitsFrom(path);
        var correctedCommits = ApplyCorrectionsTo(rawCommits, path);
        return From(correctedCommits);
    }

    private static IEnumerable<Commit> ApplyCorrectionsTo(IEnumerable<Commit> commits, string path)
    {
        foreach (var commit in commits)
        {
            var overwrite = Path.Combine(path, ".conventional-changelog", commit.Hash);
            if (File.Exists(overwrite))
                yield return new Commit(File.ReadAllText(overwrite), commit.Hash);
            else
                yield return commit;
        }
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
