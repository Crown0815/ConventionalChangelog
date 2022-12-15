using System.Linq;
using ConventionalReleaseNotes.Conventional;
using static System.Environment;

namespace ConventionalReleaseNotes.Unit.Tests;

internal static class Model
{
    internal class Changelog
    {
        private const string ChangelogTitle = "# Changelog";
        private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";

        private static readonly string HeaderSeparator = NewLine + NewLine;

        private string _text = "";

        public static Changelog Empty => new Changelog().With(ChangelogTitle + NewLine);

        public Changelog WithGroup(CommitType type, params int[] seeds) =>
            seeds.Aggregate(WithGroup(type), AddBulletPoint);

        private Changelog WithGroup(CommitType type) =>
            With(NewLine + Group(type) + HeaderSeparator);

        private static string Group(CommitType type) => $"## {type.ChangelogGroupHeader}";

        private static Changelog AddBulletPoint(Changelog c, int seed) =>
            c.With($"- {Description(seed)}{NewLine}");

        public string WithGeneralCodeImprovementsMessage() =>
            With(NewLine + GeneralCodeImprovementsMessage);

        private Changelog With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(Changelog x) => x._text;
    }

    public static string CommitWithDescription(this CommitType type, int seed) =>
        type.CommitWith(Description(seed));

    public static string CommitWith(this CommitType type, string description) =>
        $"{type.Indicator}: {description}";

    public static string WithFooter(this string commitMessage, string token, string value) =>
        commitMessage + NewLine + NewLine + $"{token}: {value}";

    public static string Description(int seed) => $"Some Description {seed}";
}
