using System;
using System.Linq;

namespace ConventionalReleaseNotes.Unit.Tests;

internal static class Model
{
    internal class Changelog
    {
        private const string ChangelogTitle = "# Changelog";
        private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";

        private static readonly string HeaderSeparator = Environment.NewLine + Environment.NewLine;

        private string _text = "";

        private static string Group(string header) => $"## {header}";

        public static Changelog Empty => new Changelog().With(ChangelogTitle + Environment.NewLine);

        public Changelog WithGroup(string header, params int[] seeds) =>
            seeds.Aggregate(WithGroup(header), (x, y) => x.WithBullet(Description(y)));

        private Changelog WithGroup(string header) => With(Environment.NewLine + Group(header) + HeaderSeparator);

        private Changelog WithBullet(string content) => With($"- {content}{Environment.NewLine}");

        public string WithGeneralCodeImprovementsMessage() => With(Environment.NewLine + GeneralCodeImprovementsMessage);

        private Changelog With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(Changelog x) => x._text;
    }

    public static string CommitWithDescription(this ConventionalCommitType type, int seed) =>
        type.CommitWith(Description(seed));

    public static string CommitWith(this ConventionalCommitType type, string description) =>
        $"{type.Indicator}: {description}";

    public static string Description(int seed) => $"Some Description {seed}";
}
