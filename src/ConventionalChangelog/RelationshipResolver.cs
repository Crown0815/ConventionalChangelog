﻿using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal record Relationship(string Token, bool DropSelf, bool DropOther);

internal class RelationshipResolver
{
    private readonly ICustomization _customization;

    public RelationshipResolver(ICustomization customization)
    {
        _customization = customization;
    }

    public IEnumerable<CommitMessage> ResolveRelationshipsBetween(IEnumerable<CommitMessage> messages)
    {
        return _customization.Relationships.Aggregate(messages, Reduce);
    }

    private static IEnumerable<CommitMessage> Reduce(IEnumerable<CommitMessage> messages, Relationship relationship)
    {
        return messages.Aggregate(new Resolver(relationship), (c, m) => c.Add(m)).Messages;
    }

    private class Resolver
    {
        private readonly record struct Rule(string Token, bool Add, bool Register, bool Remove);

        private readonly Rule _rule;
        private readonly List<CommitMessage> _messages = new();
        private readonly Dictionary<string, List<CommitMessage>> _references = new();

        public Resolver(Relationship relationship) => _rule = AsRule(relationship);

        private static Rule AsRule(Relationship relationship) => new
        (
            relationship.Token,
            relationship is { DropOther: false },
            relationship.DropSelf != relationship.DropOther,
            relationship is { DropSelf: true}
        );

        public Resolver Add(CommitMessage message)
        {
            var found = _references.TryGetValue(message.Hash, out var referenced);

            if (!found || _rule.Add) _messages.Add(message);
            if (!found || _rule.Register) CacheReferences(message);
            if (found && _rule.Remove) _messages.RemoveAll(referenced!.Contains);
            return this;
        }

        private void CacheReferences(CommitMessage source)
        {
            foreach (var target in ReferencesFrom(source))
                CacheReference(source, target);
        }

        private IEnumerable<string> ReferencesFrom(CommitMessage commitMessage) =>
            commitMessage.Footers.Where(_rule.Token.Matches).Select(Target);

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
