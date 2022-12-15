using static ConventionalReleaseNotes.Conventional.CommitMessage;

namespace ConventionalReleaseNotes.Conventional;

public record CommitMessage(CommitType Type, string Description, string Body, IReadOnlyCollection<Footer> Footers)
{
    public static CommitMessage Parse(string rawMessage) =>
        MessageParser.Parse(rawMessage);

    internal static CommitMessage Parse(Commit commit) => Parse(commit.Message) with { Hash = commit.Hash };

    public record Footer(string Token, string Value);

    public string Hash { get; private init; } = Guid.NewGuid().ToString();
}
