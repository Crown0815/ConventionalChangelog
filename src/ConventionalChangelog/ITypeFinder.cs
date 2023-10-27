using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface ITypeFinder
{
    CommitType TypeFor(string typeIndicator);
    bool IsBreakingChange(CommitMessage.Footer footer);
}
