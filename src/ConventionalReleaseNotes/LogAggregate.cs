namespace ConventionalReleaseNotes;

internal class LogAggregate
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private static readonly string EmptyChangelog = ChangelogTitle + Environment.NewLine;

    private string _text = EmptyChangelog;
    private bool _hasGeneralCodeImprovements;

    private bool IsEmpty => _text == EmptyChangelog;

    public void AddBullet(string header, string text)
    {
        if (!_text.Contains(header))
        {
            if (!IsEmpty)
                _text += Environment.NewLine;

            _text += Environment.NewLine;
            _text += ChangeGroupHeader(header) + Environment.NewLine + Environment.NewLine;
        }

        _text += BulletPoint + text + Environment.NewLine;
    }



    private static string ChangeGroupHeader(string header) => $"## {header}";

    public override string ToString()
    {
        if (IsEmpty && _hasGeneralCodeImprovements)
            return _text + Environment.NewLine + "*General Code Improvements*";
        return _text;
    }

    public void AddHidden(string _, string __) => _hasGeneralCodeImprovements = true;
}