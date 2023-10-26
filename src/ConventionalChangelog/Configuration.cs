using System.Collections.Immutable;
using System.Text.RegularExpressions;
using ConventionalChangelog.Conventional;
using static ConventionalChangelog.Conventional.Relevance;

namespace ConventionalChangelog;

public class Configuration : ITypeFinder
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

    private Configuration(IReadOnlyCollection<CommitType> commitTypes, string versionTagPrefix)
    {
        VersionTagPrefix = versionTagPrefix;
        CommitTypes = commitTypes;
        Comparer = new CommitTypeComparer(commitTypes);
    }

    private IEnumerable<CommitType> CommitTypes { get; }
    public IComparer<CommitType> Comparer { get; }
    public string VersionTagPrefix { get; }

    private class CommitTypeComparer : IComparer<CommitType>
    {
        private readonly ImmutableArray<CommitType> _map;

        public CommitTypeComparer(IEnumerable<CommitType> commitTypes)
        {
            _map = commitTypes.ToImmutableArray();
        }

        public int Compare(CommitType? x, CommitType? y) =>
            IndexOf(x).CompareTo(IndexOf(y));

        private int IndexOf(CommitType? c) => c is not null
            ? _map.IndexOf(c)
            : 0;
    }

    public CommitType TypeFor(string typeIndicator)
    {
        return CommitTypes.SingleOrDefault(x => Matches(x, typeIndicator)) ?? CommitType.None;
    }

    private bool Matches(CommitType t, string m) => Regex.IsMatch(m, $"^{t.Indicator}$");
}
