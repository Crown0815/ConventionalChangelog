namespace ConventionalChangelog;

public interface IPrintReady : IHasCommitType
{
    public string Description { get; }
}
