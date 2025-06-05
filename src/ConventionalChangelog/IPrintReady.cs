using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface IPrintReady : IHasCommitType
{
    public string Scope { get; }
    public string Description { get; }
    public string Hash { get; }
}

public interface IPrintPreparable
{
    public IPrintReady Prepare(CommitMessage context);
}
