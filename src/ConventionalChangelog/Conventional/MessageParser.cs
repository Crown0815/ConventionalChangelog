using System.Text;
using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public class MessageParser
{
    private readonly Customization _customization;

    public MessageParser() : this(new Customization(new DefaultConfiguration()))
    {
    }

    internal MessageParser(Customization customization) => _customization = customization;

    public CommitMessage Parse(Commit commit)
    {
        using var lines = new StringReader(commit.Message);
        return Read(lines, commit.Hash);
    }

    private CommitMessage Read(TextReader lines, string hash)
    {
        var (typeIndicator, description) = _customization.HeaderFrom(lines.ReadLine());
        (typeIndicator, var scope) = ScopeFrom(typeIndicator);
        var (body, footers) = BodyFrom(lines);
        typeIndicator = _customization.Sanitize(typeIndicator, footers);


        return new CommitMessage(typeIndicator, scope, description, body, footers, hash);
    }

    private static (string, string) ScopeFrom(string typeIndicator)
    {
        if (!typeIndicator.EndsWith(')')) return (typeIndicator, "");

        var indexOfOpeningParenthesis = typeIndicator.IndexOf('(');
        var scope = typeIndicator[(indexOfOpeningParenthesis + 1)..^1].Trim();
        var type = typeIndicator[..indexOfOpeningParenthesis].Trim();
        return (type, scope);
    }

    private (string, IReadOnlyCollection<Footer>) BodyFrom(TextReader reader)
    {
        var builder = new StringBuilder();
        var footers = Enumerable.Empty<Footer>();
        while (reader.ReadLine() is { } line)
        {
            if (_customization.IsFooter(line))
            {
                footers = FootersFrom(LinesFrom(reader).Prepend(line));
                break;
            }
            builder.AppendLine(line);
        }

        return (builder.ToString().Trim('\n', '\r'), footers.ToList());
    }

    private static IEnumerable<string> LinesFrom(TextReader reader)
    {
        while (reader.ReadLine() is {} line)
            yield return line;
    }

    private IEnumerable<Footer> FootersFrom(IEnumerable<string> lines)
    {
        Footer? buffer = null;
        foreach (var line in lines)
        {
            if (buffer is not null && _customization.IsFooter(line))
            {
                yield return buffer with {Value = buffer.Value.Trim()};
                buffer = null;
            }
            if (buffer is not null)
                buffer = buffer with { Value = buffer.Value + Environment.NewLine + line};
            else
                buffer = _customization.FooterFrom(line);
        }

        if (buffer is not null)
            yield return buffer with {Value = buffer.Value.Trim()};
    }
}
