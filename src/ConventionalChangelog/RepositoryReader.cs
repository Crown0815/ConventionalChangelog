using LibGit2Sharp;

namespace ConventionalChangelog;

internal class RepositoryReader
{
    private readonly ICustomization _customization;

    public RepositoryReader(ICustomization customization)
    {
        _customization = customization;
    }

    public IEnumerable<Commit> CommitsFrom(string path)
    {
        using var repository = new Repository(path);
        var newestVersionTag = NewestVersionCommitIn(repository);

        return repository.Commits
            .QueryBy(AllSince(newestVersionTag))
            .Select(c => AsCommit(c, path))
            .ToArray();
    }

    private object? NewestVersionCommitIn(IRepository repository)
    {
        var versionCommits = repository.Tags
            .Where(tag => _customization.IsVersionTag(tag.FriendlyName))
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

    private static Commit AsCommit(LibGit2Sharp.Commit commit, string path)
    {
        var message = MessageFor(commit, path);
        return new Commit(message, commit.Sha);
    }

    private static string MessageFor(LibGit2Sharp.Commit c, string path)
    {
        var overwrite = Path.Combine(path, ".conventional-changelog", c.Sha);
        if (File.Exists(overwrite))
            return File.ReadAllText(overwrite);
        return c.Message;
    }
}
