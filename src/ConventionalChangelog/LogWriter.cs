using System.Text;
using static System.Environment;

namespace ConventionalChangelog;

internal class LogWriter(Customization customization)
{
    private const string BulletPoint = "- ";
    private const string GeneralCodeImprovementsMessage = "*General Code Improvements*";
    private const string GroupHeaderPrefix = "## ";
    private const string ScopeHeaderPrefix = "### ";


    public string Print(IEnumerable<IPrintReady> writable)
    {
        var writtenLog = new WrittenLog(customization);
        foreach (var printable in customization.Ordered(writable))
            writtenLog.Add(printable);
        return writtenLog.Print();
    }

    private class WrittenLog(Customization customization)
    {
        private readonly StringBuilder _changelog = new(customization.Title);

        private string? _currentSection;
        private string? _currentSubSection;
        private bool _hasGeneralCodeImprovements;
        private bool _isEmpty = true;

        public void Add(IPrintReady printReady)
        {
            var type = customization.TypeFor(printReady.TypeIndicator);
            var scope = customization.ScopeFor(printReady.Scope);
            if (scope.GroupHeader is "")
                scope = Scope.None;
            switch (type.Relevance)
            {
                case Relevance.Show:
                    AddBullet(type.GroupHeader, scope.GroupHeader, printReady.Description);
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
            var isNewList = _currentSection != header || _currentSubSection != subHeader;
            if (_currentSection != header)
                StartNewSection(header);
            if (_currentSubSection != subHeader)
                StartNewSubSection(subHeader);
            if (isNewList)
                _changelog.AppendLine();
            AddBullet(text);
        }

        private void StartNewSection(string header)
        {
            _currentSection = header;
            _changelog.AppendFormat(null, "{0}{1}{2}{0}", NewLine, GroupHeaderPrefix, header);
        }

        private void StartNewSubSection(string? header)
        {
            _currentSubSection = header;
            _changelog.AppendFormat(null, "{0}{1}{2}{0}", NewLine, ScopeHeaderPrefix, header);
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
