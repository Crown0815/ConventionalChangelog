using ConventionalChangelog.Conventional;
using LibGit2Sharp;

namespace ConventionalChangelog;

public static class Changelog
{
    public static string From(IEnumerable<Commit> messages, IConfiguration configuration)
    {
        var logEntries = messages.Select(commit => CommitMessage.Parse(commit, configuration))
            .Reduce()
            .SelectMany(LogEntries);

        return configuration.Ordered(logEntries)
            .Aggregate(new LogAggregate(), Add).ToString();
    }

    private static IEnumerable<LogEntry> LogEntries(CommitMessage commitMessage)
    {
        foreach (var footer in commitMessage.Footers)
            if (footer.IsBreakingChange)
                yield return new LogEntry(BreakingChange.Type, footer.Value);

        yield return new LogEntry(commitMessage.Type, commitMessage.Description);
    }

    private static LogAggregate Add(LogAggregate a, LogEntry l) => a.Add(l.Type, l.Description);

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

    private record LogEntry(CommitType Type, string Description) : IHasCommitType;
}
