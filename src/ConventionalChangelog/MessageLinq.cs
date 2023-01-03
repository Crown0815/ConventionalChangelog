using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal static class MessageLinq
{
    private static readonly Strategy FixUp = new(@"fixup", true, true, true);
    private static readonly Strategy Revert = new("revert", true, false, false);
    private static readonly Strategy Override = new("override", false, false, true);

    public static IEnumerable<CommitMessage> Reduce(this IEnumerable<CommitMessage> m) => m
        .Resolve(FixUp)
        .Resolve(Revert)
        .Resolve(Override);

    private static IEnumerable<CommitMessage> Resolve(this IEnumerable<CommitMessage> messages, Strategy strategy)
    {
        var resolved = new List<CommitMessage>();
        var references = new Dictionary<string, List<CommitMessage>>();
        foreach (var message in messages) Reduce(strategy, references, message, resolved);

        return resolved;
    }

    private static void Reduce(Strategy strategy, IDictionary<string, List<CommitMessage>> references, CommitMessage message,
        List<CommitMessage> relevant)
    {
        var found = references.TryGetValue(message.Hash, out var ms);

        if (!found || strategy.Add) relevant.Add(message);
        if (!found || strategy.Register) Register(references, message, strategy.Token);
        if (found && strategy.Remove) relevant.RemoveAll(ms!.Contains);
    }

    private static void Register(IDictionary<string, List<CommitMessage>> fixUps, CommitMessage message, string token)
    {
        foreach (var target in message.Footers.Where(x => x.Token == token).Select(x => x.Value))
        {
            if (!fixUps.ContainsKey(target))
                fixUps.Add(target, new List<CommitMessage>());
            fixUps[target].Add(message);
        }
    }

    private record Strategy(string Token, bool Remove, bool Add, bool Register);
}
