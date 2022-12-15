using System;
using ConventionalReleaseNotes.Conventional;

namespace ConventionalReleaseNotes.Unit.Tests;

internal static class CommitCreationExtensions
{
    public static string CommitWithDescription(this CommitType type, int seed) =>
        type.CommitWith(A.Description(seed));

    public static string CommitWith(this CommitType type, string description) =>
        $"{type.Indicator}: {description}";

    public static string WithFooter(this string commitMessage, string token, string value) =>
        commitMessage + Environment.NewLine + Environment.NewLine + $"{token}: {value}";

    public static string WithFooter(this string commitMessage, string token, int seed) =>
        commitMessage.WithFooter(token, A.Description(seed));
}
