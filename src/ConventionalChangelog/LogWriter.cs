using System.Text;
using static System.Environment;

namespace ConventionalChangelog;

internal class LogWriter
{
    private const string BulletPoint = "- ";
    private const string ChangelogTitle = "# Changelog";
    private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";
    private const string GroupHeaderPrefix = "## ";
    private const string ScopeHeaderPrefix = "### ";

    private static readonly string EmptyChangelog = ChangelogTitle + NewLine;

    private readonly ICustomization _customization;


    public LogWriter(ICustomization customization)
    {
        _customization = customization;
    }

    public string Print(IEnumerable<IPrintReady> writable)
    {
        var writtenLog = new WrittenLog(EmptyChangelog);
        foreach (var printable in _customization.Ordered(writable))
            writtenLog.Add(printable, _customization.TypeFor(printable.TypeIndicator), printable.Scope);
        return writtenLog.Print();
    }

    private class WrittenLog
    {
        private readonly StringBuilder _changelog;

        private string? _currentSection;
        private string? _currentSubSection;
        private bool _hasGeneralCodeImprovements;
        private bool _isEmpty = true;

        public WrittenLog(string emptyChangelog)
        {
            _changelog = new StringBuilder(emptyChangelog);
        }

        public void Add(IPrintReady printReady, ChangelogType type, string? subHeader)
        {
            switch (type.Relevance)
            {
                case Relevance.Show:
                    AddBullet(type.GroupHeader, subHeader, printReady.Description);
                    break;
                case Relevance.Hide:
                    AddGeneralCodeImprovement();
                    break;
                case Relevance.Ignore:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        private void AddBullet(string header, string? subHeader, string text)
        {
            if (_currentSection != header)
                StartNewSection(header);
            if (_currentSubSection != subHeader)
                StartNewSubSection(subHeader);
            AddBullet(text);
        }

        private void StartNewSection(string header)
        {
            _currentSection = header;
            _changelog.AppendFormat(null, "{0}{1}{2}{0}{0}", NewLine, GroupHeaderPrefix, header);
        }

        private void StartNewSubSection(string? header)
        {
            _currentSubSection = header;
            _changelog.AppendFormat(null, "{1}{2}{0}{0}", NewLine, ScopeHeaderPrefix, header);
        }

        private void AddBullet(string text)
        {
            _isEmpty = false;
            _changelog.AppendLine($"{BulletPoint}{text}");
        }

        private void AddGeneralCodeImprovement() => _hasGeneralCodeImprovements = true;

        public string Print()
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
}
