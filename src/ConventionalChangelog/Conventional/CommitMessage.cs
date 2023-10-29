using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public record CommitMessage(string TypeIndicator, string Description, string Body, IReadOnlyCollection<Footer>
        Footers) : IPrintable
{
    internal static readonly CommitMessage Empty = new("", "", "", Array.Empty<Footer>());

    public record Footer(string Token, string Value);

    public string Hash { get; internal init; } = Guid.NewGuid().ToString();
}
