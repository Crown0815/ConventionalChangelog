using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal static class MessageLinq
{
    private readonly record struct Strategy(string Token, bool Remove, bool Add, bool Register);

    private static readonly Strategy[] Strategies =
    {
        new(@"fixup", true, true, true),
        new("revert", true, false, false),
        new("override", false, false, true),
    };

    public static IEnumerable<CommitMessage> Reduce(this IEnumerable<CommitMessage> messages) =>
        Strategies.Aggregate(messages, Reduce);

    private static IEnumerable<CommitMessage> Reduce(IEnumerable<CommitMessage> messages, Strategy strategy) =>
        messages.Aggregate(new Reducer(strategy), (c, m) => c.Add(m)).Messages;


    private class Reducer
    {
        private readonly Strategy _strategy;
        private readonly List<CommitMessage> _messages = new();
        private readonly Dictionary<string, List<CommitMessage>> _references = new();

        public Reducer(Strategy strategy) => _strategy = strategy;

        public Reducer Add(CommitMessage message)
        {
            var found = _references.TryGetValue(message.Hash, out var ms);

            if (!found || _strategy.Add) _messages.Add(message);
            if (!found || _strategy.Register) CacheReferences(message);
            if (found && _strategy.Remove) _messages.RemoveAll(ms!.Contains);
            return this;
        }

        private void CacheReferences(CommitMessage source)
        {
            foreach (var target in TargetsFrom(source))
                CacheReference(source, target);
        }

        private IEnumerable<string> TargetsFrom(CommitMessage source) =>
            source.Footers.Where(IsReference).Select(Target);

        private bool IsReference(CommitMessage.Footer f) => f.Token == _strategy.Token;
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
