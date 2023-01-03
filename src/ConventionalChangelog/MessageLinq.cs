using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal static class MessageLinq
{
    public static IEnumerable<CommitMessage> Reduce(this IEnumerable<CommitMessage> m) => m
        .Resolve(FixUp)
        .Resolve(Revert)
        .Resolve(Override);

    private static IEnumerable<CommitMessage> Resolve(this IEnumerable<CommitMessage> messages, Action<IDictionary<string, List<CommitMessage>>, CommitMessage, List<CommitMessage>> strategy)
    {
        var relevant = new List<CommitMessage>();
        var fixUps = new Dictionary<string, List<CommitMessage>>();
        foreach (var message in messages) strategy(fixUps, message, relevant);

        return relevant;
    }

    private static void FixUp(IDictionary<string, List<CommitMessage>> fixUps, CommitMessage message, List<CommitMessage> relevant)
    {
        if (fixUps.TryGetValue(message.Hash, out var ms))
            relevant.RemoveAll(ms.Contains);

        relevant.Add(message);
        foreach (var target in message.Footers.Where(x => x.Token == @"fixup").Select(x => x.Value))
        {
            if (!fixUps.ContainsKey(target))
                fixUps.Add(target, new List<CommitMessage>());
            fixUps[target].Add(message);
        }
    }

    private static void Revert(IDictionary<string, List<CommitMessage>> reverts, CommitMessage message, List<CommitMessage> relevant)
    {
        if (reverts.TryGetValue(message.Hash, out var reverting))
            relevant.RemoveAll(reverting.Contains);
        else
        {
            relevant.Add(message);
            foreach (var target in message.Footers.Where(x => x.Token == @"revert").Select(x => x.Value))
            {
                if (!reverts.ContainsKey(target))
                    reverts.Add(target, new List<CommitMessage>());
                reverts[target].Add(message);
            }
        }
    }

    private static void Override(IDictionary<string, List<CommitMessage>> reverts, CommitMessage message, List<CommitMessage> relevant)
    {
        if (!reverts.TryGetValue(message.Hash, out _))
            relevant.Add(message);

        foreach (var target in message.Footers.Where(x => x.Token == @"override").Select(x => x.Value))
        {
            if (!reverts.ContainsKey(target))
                reverts.Add(target, new List<CommitMessage>());
            reverts[target].Add(message);
        }
    }
}
