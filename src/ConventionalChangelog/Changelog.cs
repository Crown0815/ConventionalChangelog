using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;
using LibGit2Sharp;
using static ConventionalChangelog.ChangelogOrder;

namespace ConventionalChangelog;

public static class Changelog
{
    public static string From(IEnumerable<Commit> messages, ChangelogOrder order, Configuration configuration)
    {
        var logEntries = messages.Select(commit => CommitMessage.Parse(commit, configuration))
            .Reduce()
            .SelectMany(LogEntries);
        if (order == OldestToNewest)
            logEntries = logEntries.Reverse();

        return configuration.Ordered(logEntries)
            .Aggregate(new LogAggregate(), Add).ToString();
    }

    private static LogAggregate Add(LogAggregate a, LogEntry l) => a.Add(l.Type, l.Description);

    private static IEnumerable<LogEntry> LogEntries(CommitMessage commitMessage)
    {
        foreach (var footer in commitMessage.Footers)
            if (Regex.IsMatch(footer.Token, BreakingChange.FooterPattern))
                yield return new LogEntry(BreakingChange.Type, footer.Value);

        yield return new LogEntry(commitMessage.Type, commitMessage.Description);
    }

    public static string FromRepository(string path, Configuration configuration, ChangelogOrder order = NewestToOldest)
    {
        using var repo = new Repository(path);

        var filter = new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological,
            ExcludeReachableFrom = NewestVersionCommitIn(repo, configuration),
        };

        return From(repo.Commits.QueryBy(filter).Select(AsCommit).ToArray(), order, configuration);
    }

    private static object? NewestVersionCommitIn(IRepository repo, Configuration configuration)
    {
        var mainline = new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological,
            FirstParentOnly = true,
            IncludeReachableFrom = repo.Head,
        };

        var versionCommits = repo.Tags
            .Where(tag => configuration.IsVersionTag(tag.FriendlyName))
            .Select(x => x.Target)
            .ToHashSet();

        return repo.Commits
            .QueryBy(mainline)
            .FirstOrDefault(versionCommits.Contains);
    }

    private static Commit AsCommit(LibGit2Sharp.Commit c) => new(c.Message, c.Sha);

    private record LogEntry(CommitType Type, string Description) : IHasCommitType;
}
