using System;
using LibGit2Sharp;

namespace ConventionalReleaseNotes.Unit.Tests;

internal static class RepositoryInteractionExtensions
{
    private static readonly CommitOptions CommitOptions = new() { AllowEmptyCommit = true };
    private static readonly Identity TestIdentity = new("unit test", "unit@test.email");
    private static readonly Signature Signature = new(TestIdentity, DateTimeOffset.Now);

    public static Commit Commit(this Repository r, ConventionalCommitType type, string message)
    {
        return r.Commit(Model.ConventionalCommitMessage(type.Indicator, message));
    }

    public static Commit Commit(this Repository r, string message)
    {
        return r.Commit(message, Signature, Signature, CommitOptions);
    }
}
