using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface IConfiguration
{
    string TypeFor(string typeIndicator, IEnumerable<CommitMessage.Footer> footers);
    ChangelogType TypeFor(string typeIndicator);

    bool IsFooter(string line);
    CommitMessage.Footer FooterFrom(string line);

    IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType;

    bool IsVersionTag(string tagName);
}