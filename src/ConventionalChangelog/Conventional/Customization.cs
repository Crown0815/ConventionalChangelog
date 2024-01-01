﻿using System.Collections.Immutable;

namespace ConventionalChangelog.Conventional;

internal class Customization : ICustomization, IComparer<string>
{
    private readonly ImmutableArray<CommitType> _commitTypes;
    private readonly string _versionTagPrefix;
    private readonly ChangelogOrder _changelogOrder;
    private readonly string _footerPattern;
    private readonly string _semanticVersionPattern;

    public Customization(IConfiguration configuration)
    {
        _commitTypes = configuration.CommitTypes.ToImmutableArray();
        _versionTagPrefix = configuration.VersionTagPrefix;
        _changelogOrder = configuration.ChangelogOrder;
        _footerPattern = configuration.FooterPattern;
        _semanticVersionPattern = configuration.SemanticVersionPattern;
        Separator = configuration.HeaderTypeDescriptionSeparator;
        Relationships = new Relationship[]
        {
            new(configuration.DropSelf, true, false),
            new(configuration.DropOther, false, true),
            new(configuration.DropBoth, true, true),
        };
    }

    public string Separator { get; }

    public IReadOnlyCollection<Relationship> Relationships { get; }

    public string Sanitize(string typeIndicator, IEnumerable<CommitMessage.Footer> footers)
    {
        if (footers.OfType<IPrintReady>().SingleOrDefault() is {} p)
            return typeIndicator.ReplaceWith(InnerTypeFor(p.TypeIndicator).Indicator, "inner");
        return typeIndicator;
    }

    public ChangelogType TypeFor(string typeIndicator)
    {
        return InnerTypeFor(typeIndicator);
    }

    private CommitType InnerTypeFor(string typeIndicator)
    {
        return _commitTypes.SingleOrDefault(typeIndicator.Matches) ?? CommitType.None;
    }

    public bool IsVersionTag(string tagName) =>
        tagName.Matches(_versionTagPrefix + _semanticVersionPattern);

    public IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T : IHasCommitType
    {
        if (_changelogOrder == ChangelogOrder.OldestToNewest)
            logEntries = logEntries.Reverse();
        return logEntries.OrderBy(x => x.TypeIndicator, this);
    }

    public int Compare(string? x, string? y) => IndexOf(x).CompareTo(IndexOf(y));

    private int IndexOf(string? c) => c is not null
        ? _commitTypes.IndexOf(InnerTypeFor(c))
        : 0;

    public bool IsFooter(string line) => line.StartMatches(_footerPattern);

    public CommitMessage.Footer FooterFrom(string line)
    {
        var match = line.MatchWith(_footerPattern);
        var token = match.Groups["token"].Value;
        var value = line.Replace(match.Value, "");
        var isBreaking = match.Groups["breaking"].Success;

        return isBreaking
            ? new PrintReadyFooter(token, value, "breaking!")
            : new CommitMessage.Footer(token, value);
    }

    private record PrintReadyFooter(string Token, string Value, string TypeIndicator)
        : CommitMessage.Footer(Token, Value), IPrintReady
    {
        public string Description => Value;
    }
}
