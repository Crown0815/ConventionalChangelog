using System;

namespace ConventionalReleaseNotes.Unit.Tests;

internal class ModelChangelog
{
    private const string ChangelogHeader = "# Changelog";
    private static readonly string HeaderSeparator = Environment.NewLine + Environment.NewLine;

    private string _text = "";
    private static string Level2(string header) => $"## {header}";

    public ModelChangelog WithTitle() => With(ChangelogHeader + Environment.NewLine);

    public ModelChangelog WithGroup(string header) => With(Environment.NewLine + Level2(header) + HeaderSeparator);

    public ModelChangelog WithBullet(string content) => With($"- {content}{Environment.NewLine}");

    public string WithGeneralCodeImprovements() => With(Environment.NewLine + "*General Code Improvements*");

    private ModelChangelog With(string text)
    {
        _text += text;
        return this;
    }

    public static implicit operator string(ModelChangelog x) => x._text;
}