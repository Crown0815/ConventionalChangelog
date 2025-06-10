using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public record CommitMessage(
    string TypeIndicator,
    string Scope,
    string Description,
    string Body,
    IReadOnlyCollection<Footer> Footers,
    string Hash) : IPrintReady
{
    public record Footer(string Token, string Value);
}
