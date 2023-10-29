using ConventionalChangelog.Conventional;
using LibGit2Sharp;

namespace ConventionalChangelog;

public static class Changelog
{
    public static string From(IEnumerable<Commit> messages, IConfiguration configuration)
    {
        var logEntries = messages.Select(commit => CommitMessage.Parse(commit, configuration))
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

    public static string FromRepository(string path, IConfiguration configuration)
    {
        using var repository = new Repository(path);
        var newestVersionTag = NewestVersionCommitIn(repository, configuration);
        var filter = AllSince(newestVersionTag);

        return From(repository.Commits.QueryBy(filter).Select(AsCommit).ToArray(), configuration);
    }

    private static object? NewestVersionCommitIn(IRepository repo, IConfiguration configuration)
    {
        var mainline = MainlineFrom(repo.Head);

        var versionCommits = repo.Tags
            .Where(tag => configuration.IsVersionTag(tag.FriendlyName))
            .Select(x => x.Target)
            .ToHashSet();

        return repo.Commits
            .QueryBy(mainline)
            .FirstOrDefault(versionCommits.Contains);
    }

    private static CommitFilter MainlineFrom(Branch anchor) => new()
    {
        SortBy = CommitSortStrategies.Topological,
        FirstParentOnly = true,
        IncludeReachableFrom = anchor,
    };

    private static CommitFilter AllSince(object? anchor) => new()
    {
        SortBy = CommitSortStrategies.Topological,
        ExcludeReachableFrom = anchor,
    };

    private static Commit AsCommit(LibGit2Sharp.Commit c) => new(c.Message, c.Sha);
}
