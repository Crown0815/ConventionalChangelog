using System;
using System.Linq;
using static System.Environment;

namespace ConventionalChangelog.Unit.Tests;

internal static class A
{
    internal static ChangelogBuilder Changelog => new();

    internal class ChangelogBuilder
    {
        private const string ChangelogTitle = "# Changelog";
        private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";
        private const string HeaderL2 = "## {0}";
        private const string HeaderL3 = "### {0}";

        private static readonly string HeaderSeparator = NewLine + NewLine;

        private string _text = ChangelogTitle + NewLine;

        public ChangelogBuilder Empty => this;

        public ChangelogBuilder WithoutTitle()
        {
            var changelog = Empty;
            changelog._text = "";
            return changelog;
        }

        public string WithGeneralCodeImprovementsMessage() =>
            Empty.With(NewLine + GeneralCodeImprovementsMessage);

        public ChangelogBuilder WithGroup(ChangelogType type, params int[] seeds) =>
            Empty.And(type, seeds);

        public ChangelogBuilder WithGroup(ChangelogType type, string scope, params int[] seeds) =>
            Empty.And(type, scope, seeds);

        public ChangelogBuilder WithGroup(CommitType type, params (int, string)[] message)
        {
            return message.Aggregate(WithGroup(type.GroupHeader), AddBulletPoint);
        }

        public ChangelogBuilder And(ChangelogType type, params int[] seeds)
        {
            return seeds.Aggregate(WithGroup(type.GroupHeader), AddBulletPoint);
        }

        public ChangelogBuilder And(ChangelogType type, string scope, params int[] seeds)
        {
            return seeds.Aggregate(WithGroup(type.GroupHeader).WithScope(scope), AddBulletPoint);
        }

        private ChangelogBuilder WithGroup(string groupHeader) =>
            With(NewLine + string.Format(HeaderL2, groupHeader) + HeaderSeparator);

        private ChangelogBuilder WithScope(string scopeHeader) =>
            With(string.Format(HeaderL3, scopeHeader) + HeaderSeparator);

        public ChangelogBuilder AndScope(string scopeHeader, params int[] seeds) =>
            seeds.Aggregate(With(NewLine).WithScope(scopeHeader), AddBulletPoint);

        private static ChangelogBuilder AddBulletPoint(ChangelogBuilder c, int seed) => c.WithBulletPoint(seed);

        private static ChangelogBuilder AddBulletPoint(ChangelogBuilder c, (int seed, string hash) m) => c.WithBulletPoint(m.seed, m.hash);

        private ChangelogBuilder WithBulletPoint(int seed) => With($"- {Description(seed)}{NewLine}");
        private ChangelogBuilder WithBulletPoint(int seed, string hash) => With($"- {Description(seed)} ({hash}){NewLine}");

        private ChangelogBuilder With(string text)
        {
            _text += text;
            return this;
        }

        public static implicit operator string(ChangelogBuilder x) => x._text;
    }

    public static string Description(int seed) => $"Some Description {seed}";
    public static Commit Commit(string message) => new(message, Guid.NewGuid().ToString());
}
