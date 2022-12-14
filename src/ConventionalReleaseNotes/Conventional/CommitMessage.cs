﻿using static ConventionalReleaseNotes.Conventional.CommitMessage;

namespace ConventionalReleaseNotes.Conventional;

public record CommitMessage(CommitType Type, string Description, string Body, IReadOnlyCollection<Footer> Footers)
{
    public static CommitMessage Parse(string rawMessage) =>
        MessageParser.Parse(rawMessage);

    public record Footer(string Token, string Value);

    public string Hash { get; } = Guid.NewGuid().ToString();
}
