using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal static class MessageLinq
{
    public static IEnumerable<CommitMessage> Reduce(this IEnumerable<CommitMessage> m) => m
        .ResolveFixUps()
        .ResolveReverts();

    private static IEnumerable<CommitMessage> ResolveFixUps(this IEnumerable<CommitMessage> messages)
    {
        var relevant = new List<CommitMessage>();
        var fixUps = new Dictionary<string, List<CommitMessage>>();
        foreach (var message in messages)
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

        return relevant;
    }

    private static IEnumerable<CommitMessage> ResolveReverts(this IEnumerable<CommitMessage> messages)
    {
        var relevant = new List<CommitMessage>();
        var reverts = new Dictionary<string, List<CommitMessage>>();
        foreach (var message in messages)
        {
            if (reverts.TryGetValue(message.Hash, out var reverting))
            {
                relevant.RemoveAll(reverting.Contains);
                continue;
            }

            relevant.Add(message);
            foreach (var target in message.Footers.Where(x => x.Token == @"revert").Select(x => x.Value))
            {
                if (!reverts.ContainsKey(target))
                    reverts.Add(target, new List<CommitMessage>());
                reverts[target].Add(message);
            }
        }

        return relevant;
    }
}
