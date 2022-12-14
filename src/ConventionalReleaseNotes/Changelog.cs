using System.Text.RegularExpressions;
using ConventionalReleaseNotes.Conventional;
using LibGit2Sharp;

namespace ConventionalReleaseNotes;

public static class Changelog
{
    private const string VersionTagPrefix = "[pv]";

    public static string From(params string[] messages) => From(messages.Select(CommitMessage.Parse));

    public static string From(params CommitMessage[] messages) => From((IReadOnlyCollection<CommitMessage>)messages);

    private static string From(IEnumerable<CommitMessage> messages)
    {
        var reduced = new List<CommitMessage>();
        var hashes = messages.Select(x => x.Hash).ToList();
        foreach (var message in messages)
        {
            var fixup = message.Footers.Where(x => x.Token == @"fixup").Select(x => x.Value);
            if (hashes.Intersect(fixup).Any()) continue;
            reduced.Add(message);
        }

        return reduced
            .SelectMany(LogEntries)
            .OrderBy(x => x.Type, Configuration.Comparer)
            .Aggregate(new LogAggregate(), Add).ToString();
    }

    private static LogAggregate Add(LogAggregate a, LogEntry l) => a.Add(l.Type, l.Description);

    private static IEnumerable<LogEntry> LogEntries(CommitMessage commitMessage)
    {
        foreach (var footer in commitMessage.Footers)
            if (Regex.IsMatch(footer.Token, BreakingChange.FooterPattern))
                yield return new LogEntry(Configuration.BreakingChange, footer.Value);

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

        return From(repo.Commits.QueryBy(filter).Select(c => c.MessageShort).ToArray());
    }

    private static bool IsVersionTag(Tag tag) => tag.FriendlyName.IsSemanticVersion(VersionTagPrefix);

    private record LogEntry(CommitType Type, string Description);
}
