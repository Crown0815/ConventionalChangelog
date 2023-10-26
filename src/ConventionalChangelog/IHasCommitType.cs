using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface IHasCommitType
{
    public CommitType Type { get; }
}
