using System.Collections.Immutable;
using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration : ITypeFinder, IComparer<CommitType>
{
    private const string DefaultVersionTagPrefix = "[pv]";

    private static readonly CommitType[] DefaultCommitTypes =
    {
        BreakingChange.Type,
        new("feat", "Features", Show),
        new("fix", "Bug Fixes", Show),
        new("perf", "Performance Improvements", Show),
        new("build", "", Hide),
        new("chore", "", Hide),
        new("ci", "", Hide),
        new("docs", "", Hide),
        new("style", "", Hide),
        new("refactor", "", Hide),
        new("test", "", Hide),
    };

    public static Configuration Default() => new(DefaultCommitTypes, DefaultVersionTagPrefix);


    private readonly ImmutableArray<CommitType> _commitTypes;
    private readonly string _versionTagPrefix;

    private Configuration(IEnumerable<CommitType> commitTypes, string versionTagPrefix)
    {
        _versionTagPrefix = versionTagPrefix;
        _commitTypes = commitTypes.ToImmutableArray();
    }

    public CommitType TypeFor(string typeIndicator)
    {
        return _commitTypes.SingleOrDefault(x => Matches(x, typeIndicator)) ?? CommitType.None;
    }

    private static bool Matches(CommitType t, string m) =>
        Regex.IsMatch(m, $"^{t.Indicator}$");

    public bool IsVersionTag(string tagName) =>
        tagName.IsSemanticVersion(_versionTagPrefix);

    public IOrderedEnumerable<T> Ordered<T>(IEnumerable<T> logEntries) where T: IHasCommitType =>
        logEntries.OrderBy(x => x.Type, this);

    public int Compare(CommitType? x, CommitType? y) =>
        IndexOf(x).CompareTo(IndexOf(y));

    private int IndexOf(CommitType? c) => c is not null
        ? _commitTypes.IndexOf(c)
        : 0;
}
