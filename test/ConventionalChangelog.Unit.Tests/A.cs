using System.Linq;
using static System.Environment;

namespace ConventionalChangelog.Unit.Tests;

internal static class A
{
    internal class Changelog
    {
        private const string ChangelogTitle = "# Changelog";
        private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";

        private static readonly string HeaderSeparator = NewLine + NewLine;

        private string _text = ChangelogTitle + NewLine;

        public static Changelog Empty => new();

        public static Changelog WithoutTitle()
        {
            var changelog = Empty;
            changelog._text = "";
            return changelog;
        }

        public static string WithGeneralCodeImprovementsMessage() =>
            Empty.With(NewLine + GeneralCodeImprovementsMessage);

        public static Changelog WithGroup(ChangelogType type, params int[] seeds) =>
            Empty.And(type, seeds);

        public static Changelog WithGroup(ChangelogType type, string scope, params int[] seeds) =>
            Empty.And(type, scope, seeds);

        public Changelog And(ChangelogType type, params int[] seeds)
        {
            return seeds.Aggregate(WithGroup(type.GroupHeader), AddBulletPoint);
        }

        public Changelog And(ChangelogType type, string scope, params int[] seeds)
        {
            return seeds.Aggregate(WithGroup(type.GroupHeader).WithScope(scope), AddBulletPoint);
        }

        private Changelog WithGroup(string groupHeader) =>
            With(NewLine + Level2Header(groupHeader) + HeaderSeparator);

        private Changelog WithScope(string scopeHeader) =>
            With(Level3Header(scopeHeader) + HeaderSeparator);

        public Changelog AndScope(string scopeHeader, params int[] seeds) =>
            seeds.Aggregate(With(NewLine).WithScope(scopeHeader), AddBulletPoint);

        private static string Level2Header(string header) => $"## {header}";
        private static string Level3Header(string header) => $"### {header}";

        private static Changelog AddBulletPoint(Changelog c, int seed) => c.WithBulletPoint(seed);

        private Changelog WithBulletPoint(int seed) => With($"- {Description(seed)}{NewLine}");

        private Changelog With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(Changelog x) => x._text;
    }

    public static string Description(int seed) => $"Some Description {seed}";
}
