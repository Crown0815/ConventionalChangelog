using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public record CommitMessage(CommitType Type, string Description, string Body, IReadOnlyCollection<Footer> Footers)
{
    public static CommitMessage Parse(string rawMessage, IConfiguration configuration) =>
        MessageParser.Parse(rawMessage, configuration);

    internal static CommitMessage Parse(Commit commit, IConfiguration configuration) => Parse(commit.Message, configuration) with { Hash = commit.Hash };

    public record Footer(string Token, string Value, bool IsBreakingChange = false);

    public string Hash { get; private init; } = Guid.NewGuid().ToString();
}
