using ConventionalReleaseNotes.Conventional;
using LibGit2Sharp;

namespace ConventionalReleaseNotes;

public static class Changelog
{
    private const string VersionTagPrefix = "v";

    public static string From(params string[] commitMessages)
    {
        var messages = commitMessages.Select(CommitMessage.Parse).ToList();
        var log = new LogAggregate();

        foreach (var footer in messages.SelectMany(x => x.Footers))
        {
            if (footer.Token is "BREAKING CHANGE" or "BREAKING-CHANGE")
                log.Add(Configuration.BreakingChange, footer.Value);
        }

        foreach (var (type, description) in messages.OrderBy(x=> x.Type).Select(x => (x.Type, x.Description)))
        {
            log.Add(type, description);
        }

        return log.ToString();
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
}
