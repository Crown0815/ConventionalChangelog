using static ConventionalReleaseNotes.ConventionalCommitMessage;

namespace ConventionalReleaseNotes;

public record ConventionalCommitMessage(string Type, string Description, string Body, IReadOnlyCollection<Footer> Footers)
{
    public static ConventionalCommitMessage Parse(string rawMessage) =>
        MessageParser.Parse(rawMessage);

    public record Footer(string Token, string Value);
}
