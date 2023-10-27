using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface ITypeFinder
{
    CommitType TypeFor(string typeIndicator);

    bool IsFooter(string line);
    CommitMessage.Footer FooterFrom(string line);
    bool IsBreakingChange(CommitMessage.Footer footer);
}
