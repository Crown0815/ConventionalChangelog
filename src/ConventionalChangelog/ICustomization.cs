using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal interface ICustomization
{
    string Sanitize(string typeIndicator, IEnumerable<CommitMessage.Footer> footers);
    ChangelogType TypeFor(string typeIndicator);

    bool IsFooter(string line);
    CommitMessage.Footer FooterFrom(string line);

    IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType;

    bool IsVersionTag(string tagName);

    string Separator { get; }
    IReadOnlyCollection<Relationship> Relationships { get; }
    bool IgnoreScope { get; }
}
