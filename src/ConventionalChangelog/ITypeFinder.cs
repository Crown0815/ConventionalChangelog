using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface ITypeFinder
{
    CommitType TypeFor(string typeIndicator);
}
