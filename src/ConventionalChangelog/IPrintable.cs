namespace ConventionalChangelog;

public interface IPrintable
{
    public string TypeIndicator { get; }
    public string Description { get; }
}
