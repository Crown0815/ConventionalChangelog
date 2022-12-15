using System;
using LibGit2Sharp;
using GitCommit = LibGit2Sharp.Commit;

namespace ConventionalReleaseNotes.Unit.Tests.Integration;

internal static class RepositoryInteractionExtensions
{
    private static readonly CommitOptions CommitOptions = new() { AllowEmptyCommit = true };
    private static readonly Identity TestIdentity = new("unit test", "unit@test.email");
    private static readonly Signature Signature = new(TestIdentity, DateTimeOffset.Now);

    public static GitCommit Commit(this Repository r, Conventional.CommitType type, int seed)
    {
        return r.Commit(type, A.Description(seed));
    }

    public static GitCommit Commit(this Repository r, Conventional.CommitType type, string message)
    {
        return r.Commit(type.CommitWith(message));
    }

    public static GitCommit Commit(this Repository r, string message)
    {
        return r.Commit(message, Signature, Signature, CommitOptions);
    }

    public static void Tag(this GitObject target, string name)
    {
        target.Repository().Tags.Add(name, target);
    }

    private static IRepository Repository(this IBelongToARepository x) => x.Repository;
}
