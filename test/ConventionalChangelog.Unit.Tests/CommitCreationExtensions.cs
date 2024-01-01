using System;

namespace ConventionalChangelog.Unit.Tests;

internal static class CommitCreationExtensions
{
    public static Commit CommitWithDescription(this CommitType type, int seed) =>
        type.CommitWith(A.Description(seed));

    public static Commit CommitWith(this CommitType type, string description) =>
        new($"{type.Indicator}: {description}");

    public static Commit WithFooter(this Commit commitMessage, string token, int seed) =>
        commitMessage.WithFooter(token, A.Description(seed));

    public static Commit WithFooter(this Commit commitMessage, string token, string value) =>
        commitMessage with {Message = commitMessage.Message + Environment.NewLine + Environment.NewLine + $"{token}: {value}"};
}
