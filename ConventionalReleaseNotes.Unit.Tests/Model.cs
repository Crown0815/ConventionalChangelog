using System;

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

        public Changelog WithGroup(string header) => With(Environment.NewLine + Group(header) + HeaderSeparator);

        public Changelog WithBullet(string content) => With($"- {content}{Environment.NewLine}");

        public string WithGeneralCodeImprovementsMessage() => With(Environment.NewLine + GeneralCodeImprovementsMessage);

        private Changelog With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(Changelog x) => x._text;
    }

    public static string ConventionalCommitMessage(string typeIndicator, string summary) =>
        $"{typeIndicator}: {summary}";

    public static string Message(int seed) => $"Some Message {seed}";
}
