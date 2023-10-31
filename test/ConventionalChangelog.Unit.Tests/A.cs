using System.Linq;
using ConventionalChangelog.Conventional;
using static System.Environment;

namespace ConventionalChangelog.Unit.Tests;

internal static class A
{
    internal class Changelog
    {
        private static readonly ConventionalChangelog.Changelog ConventionalChangelog = new(Configured.Default());

        public static string From(params Commit[] messages) =>
            ConventionalChangelog.From(messages);

        private const string ChangelogTitle = "# Changelog";
        private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";

        private static readonly string HeaderSeparator = NewLine + NewLine;

        private string _text = ChangelogTitle + NewLine;

        public static Changelog Empty => new();

        public static string WithGeneralCodeImprovementsMessage() =>
            Empty.With(NewLine + GeneralCodeImprovementsMessage);

        public static Changelog WithGroup(ChangelogType type, params int[] seeds) =>
            Empty.And(type, seeds);

        public Changelog And(ChangelogType type, params int[] seeds)
        {
            return seeds.Aggregate(WithGroup(type.GroupHeader), AddBulletPoint);
        }

        private Changelog WithGroup(string groupHeader) =>
            With(NewLine + Group(groupHeader) + HeaderSeparator);

        private static string Group(string groupHeader) => $"## {groupHeader}";

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
