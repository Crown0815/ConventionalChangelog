using System.Text;
using ConventionalChangelog.Conventional;
using static System.Environment;

namespace ConventionalChangelog;

internal class LogAggregate
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";
    private const string GroupHeaderPrefix = "## ";

    private static readonly string EmptyChangelog = ChangelogTitle + NewLine;

    private readonly StringBuilder _changelog = new(EmptyChangelog);
    private bool _hasGeneralCodeImprovements;
    private readonly IConfigured _configured;
    private string? _currentGroup;
    private bool _isEmpty = true;

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
        _isEmpty = false;
        if (_currentGroup != header)
            StartNewGroup(header);
        _changelog.AppendLine($"{BulletPoint}{text}");
    }

    private void StartNewGroup(string header)
    {
        _currentGroup = header;
        _changelog.AppendFormat(null, "{0}{1}{2}{0}{0}", NewLine, GroupHeaderPrefix, header);
    }

    private void AddHidden() => _hasGeneralCodeImprovements = true;

    public override string ToString()
    {
        if (_isEmpty && _hasGeneralCodeImprovements)
            return GeneralCodeImprovements().ToString();
        return _changelog.ToString();
    }

    private StringBuilder GeneralCodeImprovements()
    {
        _changelog.AppendLine();
        _changelog.Append(GeneralCodeImprovementsMessage);
        return _changelog;
    }
}
