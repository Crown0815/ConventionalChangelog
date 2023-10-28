using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface IConfiguration
{
    CommitType TypeFor(string typeIndicator);

    bool IsFooter(string line);
    CommitMessage.Footer FooterFrom(string line);
    bool IsBreakingChange(CommitMessage.Footer footer);

    IEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType;

    bool IsVersionTag(string tagName);
}
