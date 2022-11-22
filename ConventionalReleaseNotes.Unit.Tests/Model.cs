using System;

namespace ConventionalReleaseNotes.Unit.Tests;

internal static class Model
{
    internal class Changelog
    {
        private const string ChangelogHeader = "# Changelog";
        private static readonly string HeaderSeparator = Environment.NewLine + Environment.NewLine;

        private string _text = "";
        private static string Level2(string header) => $"## {header}";

        public Changelog WithTitle() => With(ChangelogHeader + Environment.NewLine);

        public Changelog WithGroup(string header) => With(Environment.NewLine + Level2(header) + HeaderSeparator);

        public Changelog WithBullet(string content) => With($"- {content}{Environment.NewLine}");

        public string WithGeneralCodeImprovements() => With(Environment.NewLine + "*General Code Improvements*");

        private Changelog With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(Changelog x) => x._text;
    }
}
