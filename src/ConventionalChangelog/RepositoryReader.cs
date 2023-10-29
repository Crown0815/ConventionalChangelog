using LibGit2Sharp;

namespace ConventionalChangelog;

public static class RepositoryReader
{
    public static IEnumerable<Commit> CommitsFrom(string path, IConfiguration configuration)
    {
        using var repository = new Repository(path);
        var newestVersionTag = NewestVersionCommitIn(repository, configuration);

        return repository.Commits
            .QueryBy(AllSince(newestVersionTag))
            .Select(AsCommit)
            .ToArray();
    }

    private static object? NewestVersionCommitIn(IRepository repository, IConfiguration configuration)
    {
        var versionCommits = repository.Tags
            .Where(tag => configuration.IsVersionTag(tag.FriendlyName))
            .Select(x => x.Target)
            .ToHashSet();

        return repository.Commits
            .QueryBy(MainlineFrom(repository.Head))
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
