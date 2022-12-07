using System.Text.RegularExpressions;
using ConventionalReleaseNotes.Conventional;
using LibGit2Sharp;

namespace ConventionalReleaseNotes;

public static class Changelog
{
    private const string VersionTagPrefix = "v";

    private static readonly CommitType[] CommitTypes =
    {
        new("[a-z]+!", "Breaking Changes"),
        new("feat", "Features"),
        new("fix", "Bug Fixes"),
        new("perf", "Performance Improvements"),
        new("[a-z]+", "", true),
    };

    public static string From(params string[] commitMessages)
    {
        var messages = commitMessages.Select(CommitMessage.Parse).ToList();
        var log = new LogAggregate();

        foreach (var footer in messages.SelectMany(x => x.Footers))
        {
            if (footer.Token is "BREAKING CHANGE" or "BREAKING-CHANGE")
                log.AddBullet("Breaking Changes", footer.Value);
        }

        foreach (var type in CommitTypes)
        foreach (var message in messages.Where(x => type.Matches(x.Type)))
        {
            if (type.HideFromChangelog)
                log.AddHidden(type.Header, message.Description);
            else
                log.AddBullet(type.Header, message.Description);
        }

        return log.ToString();
    }

    private static bool Matches(this CommitType t, string m) => Regex.IsMatch(m, $"^{t.Indicator}$");

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
