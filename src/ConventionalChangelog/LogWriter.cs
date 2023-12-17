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

    private readonly ICustomization _customization;


    public LogWriter(ICustomization customization)
    {
        _customization = customization;
    }

    public string Print(IEnumerable<IPrintable> writable)
    {
        var writtenLog = new WrittenLog(EmptyChangelog);
        foreach (var printable in _customization.Ordered(writable))
            writtenLog.Add(printable, _customization.TypeFor(printable.TypeIndicator));
        return writtenLog.Print();
    }

    private class WrittenLog
    {
        private readonly StringBuilder _changelog;

        private string? _currentSection;
        private bool _hasGeneralCodeImprovements;
        private bool _isEmpty = true;

        public WrittenLog(string emptyChangelog)
        {
            _changelog = new StringBuilder(emptyChangelog);
        }

        public void Add(IPrintable printable, ChangelogType type)
        {
            if (type.Relevance == Relevance.Show)
                AddBullet(type.GroupHeader, printable.Description);
            if (type.Relevance == Relevance.Hide)
                AddGeneralCodeImprovement();
        }

        private void AddBullet(string header, string text)
        {
            if (_currentSection != header)
                StartNewSection(header);
            AddBullet(text);
        }

        private void StartNewSection(string header)
        {
            _currentSection = header;
            _changelog.AppendFormat(null, "{0}{1}{2}{0}{0}", NewLine, GroupHeaderPrefix, header);
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
