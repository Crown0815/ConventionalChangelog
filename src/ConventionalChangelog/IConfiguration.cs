using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface IConfiguration
{
    CommitType TypeFor(string typeIndicator, IReadOnlyCollection<CommitMessage.Footer> footers);

    bool IsFooter(string line);
    CommitMessage.Footer FooterFrom(string line);

    IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType;

    bool IsVersionTag(string tagName);
}
