using ConventionalReleaseNotes.Conventional;
using LibGit2Sharp;

namespace ConventionalReleaseNotes;

public static class Changelog
{
    private const string VersionTagPrefix = "v";

    public static string From(params string[] commitMessages)
    {
        var messages = commitMessages.Select(CommitMessage.Parse);
        var logEntries = messages.SelectMany(LogEntries);
        var ordered = logEntries.OrderBy(x => x.Type, Configuration.Comparer);
        var aggregate = ordered.Aggregate(new LogAggregate(), Add);

        return aggregate.ToString();
    }

    private static LogAggregate Add(LogAggregate a, LogEntry l) => a.Add(l.Type, l.Description);

    private static IEnumerable<LogEntry> LogEntries(CommitMessage commitMessage)
    {
        foreach (var footer in commitMessage.Footers)
            if (footer.Token is "BREAKING CHANGE" or "BREAKING-CHANGE")
                yield return new LogEntry(Configuration.BreakingChange, footer.Value);

        yield return new LogEntry(commitMessage.Type, commitMessage.Description);
    }

    public static string FromRepository(string path)
    {
        using var repo = new Repository(path);
        var tag = repo.Tags.Where(IsVersionTag).LastOrDefault();
        var filter = new CommitFilter {
            SortBy = CommitSortStrategies.Topological,
            ExcludeReachableFrom = tag,
        };

        return From(repo.Commits.QueryBy(filter).Select(c => c.MessageShort).ToArray());
    }

    private static bool IsVersionTag(Tag tag)
    {
        var tagNameWithoutVersionPrefix = tag.FriendlyName.Replace(VersionTagPrefix, "");
        return tagNameWithoutVersionPrefix.IsSemanticVersion();
    }

    private record LogEntry(CommitType Type, string Description);
}
