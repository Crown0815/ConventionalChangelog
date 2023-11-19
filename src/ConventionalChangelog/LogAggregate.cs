using ConventionalChangelog.Conventional;

namespace ConventionalChangelog;

internal class LogAggregate
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";
    private const string GroupHeaderPrefix = "## ";

    private static readonly string EmptyChangelog = ChangelogTitle + Environment.NewLine;

    private string _text = EmptyChangelog;
    private bool _hasGeneralCodeImprovements;
    private readonly IConfigured _configured;

    public LogAggregate(IConfigured configured)
    {
        _configured = configured;
    }

    public LogAggregate Add(string typeIndicator, string description)
    {
        var type = _configured.TypeFor(typeIndicator);
        if (type.Relevance == Relevance.Show) AddBullet(type.GroupHeader, description);
        if (type.Relevance == Relevance.Hide) AddHidden();

        return this;
    }

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

    private void AddHidden() => _hasGeneralCodeImprovements = true;

    public override string ToString()
    {
        if (IsEmpty && _hasGeneralCodeImprovements)
            return _text + Environment.NewLine + GeneralCodeImprovementsMessage;
        return _text;
    }

    private bool IsEmpty => _text == EmptyChangelog;
}
