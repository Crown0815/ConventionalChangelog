    using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace ConventionalReleaseNotes;

public static class Changelog
{
    private static readonly ConventionalCommitType[] CommitTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
        new("[a-z]+: ", "", true),
    };

    public static string From(params string[] commitMessages)
    {
        var log = new LogAggregate();

        foreach (var type in CommitTypes)
        foreach (var message in commitMessages.Where(type.Matches))
        {
            if (type.HideFromChangelog)
                log.AddHidden(type.Header, type.MessageFrom(message));
            else
                log.AddBullet(type.Header, type.MessageFrom(message));
        }

        return log.ToString();
    }

    private static string MessageFrom(this ConventionalCommitType t, string m) => Regex.Replace(m, t.Indicator, "");
    private static bool Matches(this ConventionalCommitType t, string m) => Regex.IsMatch(m, $"{t.Indicator}.+");

    public static string FromRepository(string path)
    {
        using var repo = new Repository(path);
        var tag = repo.Tags.LastOrDefault();
        var filter = new CommitFilter {
            SortBy = CommitSortStrategies.Topological,
            ExcludeReachableFrom = tag,
        };

        return From(repo.Commits.QueryBy(filter).Select(c => c.MessageShort).ToArray());
    }
}
