using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal class RelationshipResolver
{
    private readonly record struct Relationship(string Token, bool Add, bool Register, bool Remove);

    private readonly IEnumerable<Relationship> _relationships;

    public RelationshipResolver(Configuration configuration)
    {
        _relationships = new Relationship[] {
            new(configuration.DropSelf, true, true, true),
            new(configuration.DropBoth, false, false, true),
            new(configuration.DropOther, false, true, false),
        };
    }

    public IEnumerable<CommitMessage> ResolveRelationshipsBetween(IEnumerable<CommitMessage> messages)
    {
        return _relationships.Aggregate(messages, Reduce);
    }

    private static IEnumerable<CommitMessage> Reduce(IEnumerable<CommitMessage> messages, Relationship relationship)
    {
        return messages.Aggregate(new Resolver(relationship), (c, m) => c.Add(m)).Messages;
    }


    private class Resolver
    {
        private readonly Relationship _relationship;
        private readonly List<CommitMessage> _messages = new();
        private readonly Dictionary<string, List<CommitMessage>> _references = new();

        public Resolver(Relationship relationship) => _relationship = relationship;

        public Resolver Add(CommitMessage message)
        {
            var found = _references.TryGetValue(message.Hash, out var referenced);

            if (!found || _relationship.Add) _messages.Add(message);
            if (!found || _relationship.Register) CacheReferences(message);
            if (found && _relationship.Remove) _messages.RemoveAll(referenced!.Contains);
            return this;
        }

        private void CacheReferences(CommitMessage source)
        {
            foreach (var target in ReferencesFrom(source))
                CacheReference(source, target);
        }

        private IEnumerable<string> ReferencesFrom(CommitMessage commitMessage) =>
            commitMessage.Footers.Where(_relationship.Token.Matches).Select(Target);

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
