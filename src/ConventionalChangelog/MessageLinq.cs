using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal static class MessageLinq
{
    public static IEnumerable<CommitMessage> Reduce(this IEnumerable<CommitMessage> m) => m
        .ResolveFixUps();

    private static IEnumerable<CommitMessage> ResolveFixUps(this IEnumerable<CommitMessage> messages)
    {
        var relevant = new List<CommitMessage>();
        var fixUps = new Dictionary<string, List<CommitMessage>>();
        foreach (var message in messages)
        {
            relevant.Add(message);
            foreach (var target in message.Footers.Where(x => x.Token == @"fixup").Select(x => x.Value))
            {
                if (!fixUps.ContainsKey(target))
                    fixUps.Add(target, new List<CommitMessage>());
                fixUps[target].Add(message);
            }

            if (fixUps.TryGetValue(message.Hash, out var ms))
                relevant.RemoveAll(ms.Contains);
        }

        return relevant;
    }
}
