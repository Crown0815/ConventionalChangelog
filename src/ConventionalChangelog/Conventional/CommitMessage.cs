﻿using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public record CommitMessage(string TypeIndicator, string Description, string Body, IReadOnlyCollection<Footer>
        Footers) : IPrintable
{
    public static CommitMessage Parse(string rawMessage, IConfiguration configuration) =>
        MessageParser.Parse(rawMessage, configuration);

    public record Footer(string Token, string Value);

    public string Hash { get; internal init; } = Guid.NewGuid().ToString();
}
