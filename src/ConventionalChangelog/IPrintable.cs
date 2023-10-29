namespace ConventionalChangelog;

public interface IPrintable : IHasCommitType
{
    public string Description { get; }
}
