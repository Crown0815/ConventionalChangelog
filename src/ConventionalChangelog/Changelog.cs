using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;
using LibGit2Sharp;
using static ConventionalChangelog.Configuration;

namespace ConventionalChangelog;

public static class Changelog
{
    public static string From(params Commit[] messages) => From(messages.Select(CommitMessage.Parse));

    private static string From(IEnumerable<CommitMessage> messages)
    {
        return messages
            .Reduce()
            .SelectMany(LogEntries)
            .OrderBy(x => x.Type, Comparer)
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

    public static string FromRepository(string path)
    {
        using var repo = new Repository(path);
        var dict = repo.Tags.GroupBy(x=> x.Target).ToDictionary(x => x.Key, x => x.ToList());

        var tag = (object)null!;


        var filter0 = new CommitFilter {
            SortBy = CommitSortStrategies.Topological,
            IncludeReachableFrom = repo.Head,
        };

        foreach (var commit in repo.Commits.QueryBy(filter0))
        {
            if (!dict.TryGetValue(commit, out var t) || !t.Any(IsVersionTag))
                continue;
            tag = commit;
            break;
        }

        var filter = new CommitFilter {
            SortBy = CommitSortStrategies.Topological,
            ExcludeReachableFrom = tag,
        };

        return From(repo.Commits.QueryBy(filter).Select(AsCommit).ToArray());
    }

    private static Commit AsCommit(LibGit2Sharp.Commit c) => new(c.Message, c.Sha);

    private static bool IsVersionTag(Tag tag) => tag.FriendlyName.IsSemanticVersion(VersionTagPrefix);

    private record LogEntry(CommitType Type, string Description);
}
