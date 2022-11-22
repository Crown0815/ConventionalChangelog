using System.Text.RegularExpressions;
using static System.Environment;

namespace ConventionalReleaseNotes;

internal class LogAggregate
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private static readonly string EmptyChangelog = ChangelogTitle + NewLine;

    private string _text = EmptyChangelog;
    private bool _hasGeneralCodeImprovements;

    private bool IsEmpty => _text == EmptyChangelog;

    public void AddBullet(string header, string text)
    {
        if (!_text.Contains(header))
        {
            if (!IsEmpty)
                _text += NewLine;

            _text += NewLine;
            _text += ChangeGroupHeader(header) + NewLine + NewLine;
        }

        _text += BulletPoint + text + NewLine;
    }



    private static string ChangeGroupHeader(string header) => $"## {header}";

    public override string ToString()
    {
        if (IsEmpty && _hasGeneralCodeImprovements)
            return _text + NewLine + "*General Code Improvements*";
        return _text;
    }

    public void AddHidden(string _, string __) => _hasGeneralCodeImprovements = true;
}


public static class Changelog
{
    private static readonly ConventionalCommitType[] CommitTypes =
    {
        new("feat: ", "Features"),
        new("fix: ", "Bug Fixes"),
        new("perf: ", "Performance Improvements"),
        new("[a-z]+: ", "", true),
    };

    public static string From(params string[] commitMessages)
    {
        var log = new LogAggregate();

        foreach (var type in CommitTypes)
        foreach (var message in commitMessages.Where(type.Matches))
        {
            if (type.HideFromChangelog)
                log.AddHidden(type.Header, type.MessageFrom(message));
            else
                log.AddBullet(type.Header, type.MessageFrom(message));
        }

        return log.ToString();
    }

    private static string MessageFrom(this ConventionalCommitType t, string m) => Regex.Replace(m, t.Indicator, "");
    private static bool Matches(this ConventionalCommitType t, string m) => Regex.IsMatch(m, $"{t.Indicator}.+");
}
