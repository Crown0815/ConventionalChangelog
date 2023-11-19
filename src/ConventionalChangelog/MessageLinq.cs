using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal class MessageLinq
{
    private readonly record struct Strategy(string Token, bool Add, bool Register, bool Remove);

    private readonly IEnumerable<Strategy> _strategies;

    public MessageLinq(Configuration configuration)
    {
        _strategies = new Strategy[] {
            new(configuration.DropSelf, true, true, true),
            new(configuration.DropBoth, false, false, true),
            new(configuration.DropOther, false, true, false),
        };
    }

    public IEnumerable<CommitMessage> Reduce(IEnumerable<CommitMessage> messages)
    {
        return _strategies.Aggregate(messages, Reduce);
    }

    private static IEnumerable<CommitMessage> Reduce(IEnumerable<CommitMessage> messages, Strategy strategy)
    {
        return messages.Aggregate(new Reducer(strategy), (c, m) => c.Add(m)).Messages;
    }


    private class Reducer
    {
        private readonly Strategy _strategy;
        private readonly List<CommitMessage> _messages = new();
        private readonly Dictionary<string, List<CommitMessage>> _references = new();

        public Reducer(Strategy strategy) => _strategy = strategy;

        public Reducer Add(CommitMessage message)
        {
            var found = _references.TryGetValue(message.Hash, out var referenced);

            if (!found || _strategy.Add) _messages.Add(message);
            if (!found || _strategy.Register) CacheReferences(message);
            if (found && _strategy.Remove) _messages.RemoveAll(referenced!.Contains);
            return this;
        }

        private void CacheReferences(CommitMessage source)
        {
            foreach (var target in ReferencesFrom(source))
                CacheReference(source, target);
        }

        private IEnumerable<string> ReferencesFrom(CommitMessage commitMessage) =>
            commitMessage.Footers.Where(_strategy.Token.Matches).Select(Target);

        private static string Target(CommitMessage.Footer f) => f.Value;

        private void CacheReference(CommitMessage source, string target)
        {
            if (!_references.ContainsKey(target))
                _references.Add(target, new List<CommitMessage>());
            _references[target].Add(source);
        }

        public IEnumerable<CommitMessage> Messages => _messages;
    }
}
