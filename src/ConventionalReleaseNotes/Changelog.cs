using System.Text.RegularExpressions;
using ConventionalReleaseNotes.Conventional;
using LibGit2Sharp;

namespace ConventionalReleaseNotes;

public static class Changelog
{
    private const string VersionTagPrefix = "v";

    public static string From(params string[] commitMessages) => commitMessages
        .SelectMany(LogEntries)
        .OrderBy(x => x.Type, Configuration.Comparer)
        .Aggregate(new LogAggregate(), Add).ToString();

    private static LogAggregate Add(LogAggregate a, LogEntry l) => a.Add(l.Type, l.Description);

    private static IEnumerable<LogEntry> LogEntries(string rawMessage) =>
        LogEntries(CommitMessage.Parse(rawMessage));

    private static IEnumerable<LogEntry> LogEntries(CommitMessage commitMessage)
    {
        foreach (var footer in commitMessage.Footers)
            if (Regex.IsMatch(footer.Token, Pattern.BreakingChange))
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
