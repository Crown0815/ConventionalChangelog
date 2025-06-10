using static System.Environment;

namespace ConventionalChangelog.Unit.Tests;

internal static class CommitCreationExtensions
{
    private const char TypeDescriptionSeparator = ':';
    private const char TokenValueSeparator = TypeDescriptionSeparator;
    private static readonly string BlankLine = NewLine + NewLine;

    public static Commit CommitWithDescription(this CommitType type, int seed) =>
        type.CommitWith(A.Description(seed));

    public static Commit CommitWith(this CommitType type, string description) =>
        A.Commit($"{type.Indicator}{TypeDescriptionSeparator} {description}");

    public static Commit WithFooter(this Commit commitMessage, string token, int seed) =>
        commitMessage.WithFooter(token, A.Description(seed));

    public static Commit WithFooter(this Commit commitMessage, string token, string value) =>
        commitMessage with {Message = commitMessage.Message + BlankLine + $"{token}{TokenValueSeparator} {value}"};

    public static Commit WithScope(this Commit commitMessage, string scope) =>
        commitMessage with {Message = commitMessage.Message.Insert(commitMessage.Message.IndexOf(TypeDescriptionSeparator), $"({scope})")};
}
