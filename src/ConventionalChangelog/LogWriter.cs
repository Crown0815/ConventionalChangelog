using System.Text;
using ConventionalChangelog.Conventional;
using static System.Environment;

namespace ConventionalChangelog;

internal class LogWriter
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

    public LogWriter(IConfigured configured)
    {
        _configured = configured;
    }

    public string Write(IEnumerable<IPrintable> writable)
    {
        foreach (var printable in _configured.Ordered(writable))
            Add(printable);
        return Write();
    }

    private void Add(IPrintable printable)
    {
        var type = _configured.TypeFor(printable.TypeIndicator);
        if (type.Relevance == Relevance.Show) AddBullet(type.GroupHeader, printable.Description);
        if (type.Relevance == Relevance.Hide) AddHidden();
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

    private string Write()
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
