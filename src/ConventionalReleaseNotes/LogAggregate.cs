using ConventionalReleaseNotes.Conventional;

namespace ConventionalReleaseNotes;

internal class LogAggregate
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";
    private const string GroupHeaderPrefix = "## ";

    private static readonly string EmptyChangelog = ChangelogTitle + Environment.NewLine;

    private string _text = EmptyChangelog;
    private bool _hasGeneralCodeImprovements;

    private bool IsEmpty => _text == EmptyChangelog;

    private void AddBullet(string header, string text)
    {
        if (!_text.Contains(header))
        {
            _text += Environment.NewLine;
            _text += ChangeGroupHeader(header) + Environment.NewLine + Environment.NewLine;
        }

        _text += BulletPoint + text + Environment.NewLine;
    }

    private static string ChangeGroupHeader(string header) => GroupHeaderPrefix + header;

    public override string ToString()
    {
        if (IsEmpty && _hasGeneralCodeImprovements)
            return _text + Environment.NewLine + GeneralCodeImprovementsMessage;
        return _text;
    }

    private void AddHidden() => _hasGeneralCodeImprovements = true;

    public void Add(CommitType type, string description)
    {
        switch (type.Relevance)
        {
            case Relevance.Show:
                AddBullet(type.ChangelogGroupHeader, description);
                break;
            case Relevance.Hide:
                AddHidden();
                break;
            case Relevance.Ignore:
            default:
                return;
        }
    }
}
