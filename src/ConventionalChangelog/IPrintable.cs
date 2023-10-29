using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

public interface IPrintable
{
    public CommitType Type { get; }
    public string Description { get; }
}
