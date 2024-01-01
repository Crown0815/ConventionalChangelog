using System;
using LibGit2Sharp;
using GitCommit = LibGit2Sharp.Commit;

namespace ConventionalChangelog.Unit.Tests.Integration;

internal static class RepositoryInteractionExtensions
{
    private static readonly CommitOptions CommitOptions = new() { AllowEmptyCommit = true };
    private static readonly Identity TestIdentity = new("unit test", "unit@test.email");
    private static readonly Signature Signature = new(TestIdentity, DateTimeOffset.Now);

    public static GitCommit Commit(this Repository r, CommitType type, int seed)
    {
        return r.Commit(type, A.Description(seed));
    }

    public static GitCommit Commit(this Repository r, CommitType type, string message)
    {
        return r.Commit(type.CommitWith(message));
    }

    private static GitCommit Commit(this Repository r, Commit message)
    {
        return r.Commit(message.Message);
    }

    public static GitCommit Commit(this Repository r, string message)
    {
        return r.Commit(message, Signature, Signature, CommitOptions);
    }

    public static void Tag(this GitObject target, string name)
    {
        target.Repository().Tags.Add(name, target);
    }

    public static void Checkout(this IRepository r, string id)
    {
        Commands.Checkout(r, id);
    }

    public static void Checkout(this IRepository r, GitCommit commit)
    {
        Commands.Checkout(r, commit);
    }

    public static GitCommit Merge(this Repository r, Branch branch)
    {
        return r.Merge(branch, Signature, new MergeOptions {FastForwardStrategy = FastForwardStrategy.NoFastForward}).Commit;
    }

    private static IRepository Repository(this IBelongToARepository x) => x.Repository;

    public static string Path(this Repository r) => r.Info.WorkingDirectory;
}
