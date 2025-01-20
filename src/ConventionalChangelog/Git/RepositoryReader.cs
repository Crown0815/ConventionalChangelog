using LibGit2Sharp;

namespace ConventionalChangelog.Git;

internal class RepositoryReader(Customization customization)
{
    public IEnumerable<Commit> CommitsFrom(string path)
    {
        using var repository = new RepositoryWrapper(path);
        return repository.AllCommitsSinceLastTagMatching(customization.IsVersionTag);
    }


    private class RepositoryWrapper(string path) : IDisposable
    {
        private readonly Repository _inner = new(path);

        public Commit[] AllCommitsSinceLastTagMatching(Func<string, bool> condition)
        {
            var lastTaggedCommit = FirstCommitFromHeadWithTagMatching(condition);
            return AllCommitsSince(lastTaggedCommit);
        }

        private LibGit2Sharp.Commit? FirstCommitFromHeadWithTagMatching(Func<string, bool> condition)
        {
            var taggedCommits = CommitsWithTagMatching(condition);
            return FirstCommitFromHeadContainedIn(taggedCommits);
        }

        private HashSet<GitObject> CommitsWithTagMatching(Func<string, bool> predicate)
        {
            return _inner.Tags
                .Where(t => predicate(t.FriendlyName))
                .Select(t => t.Target)
                .ToHashSet();
        }

        private LibGit2Sharp.Commit? FirstCommitFromHeadContainedIn(IReadOnlySet<GitObject> commits)
        {
            return _inner.Commits
                .QueryBy(MainlineFrom(_inner.Head))
                .FirstOrDefault(commits.Contains);
        }

        private static CommitFilter MainlineFrom(Branch anchor) => new()
        {
            SortBy = CommitSortStrategies.Topological,
            FirstParentOnly = true,
            IncludeReachableFrom = anchor,
        };

        private Commit[] AllCommitsSince(object? referenceCommit)
        {
            return _inner.Commits
                .QueryBy(AllSince(referenceCommit))
                .Select(AsCommit)
                .ToArray();
        }

        private static CommitFilter AllSince(object? anchor) => new()
        {
            SortBy = CommitSortStrategies.Topological,
            ExcludeReachableFrom = anchor,
        };

        private Commit AsCommit(LibGit2Sharp.Commit commit)
        {
            var message = MessageFor(commit);
            return new Commit(message, commit.Sha);
        }

        private string MessageFor(LibGit2Sharp.Commit c)
        {
            var overwrite = Path.Combine(path, ".conventional-changelog", c.Sha);
            return File.Exists(overwrite)
                ? File.ReadAllText(overwrite)
                : c.Message;
        }

        public void Dispose() => _inner.Dispose();
    }
}
