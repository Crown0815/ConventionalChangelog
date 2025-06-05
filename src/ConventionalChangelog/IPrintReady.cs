namespace ConventionalChangelog;

public interface IPrintReady : IHasCommitType
{
    public string Scope { get; }
    public string Description { get; }
    public string Hash { get; }
}

public interface IPrintRelevant
{
    public IPrintReady Prepare(string hash);
}
