using System.Linq;
using ConventionalChangelog.Conventional;
using static System.Environment;
using static ConventionalChangelog.ChangelogOrder;

namespace ConventionalChangelog.Unit.Tests;

internal static class A
{
    internal class Changelog
    {
        public static string From(params Commit[] messages) =>
            ConventionalChangelog.Changelog.From(messages, NewestToOldest, Configuration.Default());

        private const string ChangelogTitle = "# Changelog";
        private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";

        private static readonly string HeaderSeparator = NewLine + NewLine;

        private string _text = ChangelogTitle + NewLine;

        public static Changelog Empty => new();

        public static string WithGeneralCodeImprovementsMessage() =>
            new Changelog().With(NewLine + GeneralCodeImprovementsMessage);

        public static Changelog WithGroup(CommitType type, params int[] seeds) =>
            new Changelog().And(type, seeds);

        public Changelog And(CommitType type, params int[] seeds) =>
            seeds.Aggregate(WithGroup(type), AddBulletPoint);

        private Changelog WithGroup(CommitType type) =>
            With(NewLine + Group(type) + HeaderSeparator);

        private static string Group(CommitType type) => $"## {type.ChangelogGroupHeader}";

        private static Changelog AddBulletPoint(Changelog c, int seed) =>
            c.With($"- {Description(seed)}{NewLine}");

        private Changelog With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(Changelog x) => x._text;
    }

    public static string Description(int seed) => $"Some Description {seed}";
}
