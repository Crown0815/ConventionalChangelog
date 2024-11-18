using System.Text;
using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public class MessageParser
{
    private readonly ICustomization _customization;

    public MessageParser() : this(new Customization(new DefaultConfiguration()))
    {
    }

    internal MessageParser(ICustomization customization) => _customization = customization;

    public CommitMessage Parse(Commit commit)
    {
        return Parse(commit.Message) with { Hash = commit.Hash };
    }

    public CommitMessage Parse(string rawMessage)
    {
        using var lines = new StringReader(rawMessage);
        return Read(lines);
    }

    private CommitMessage Read(TextReader lines)
    {
        var (typeIndicator, description) = HeaderFrom(lines.ReadLine());
        (typeIndicator, var scope) = ScopeFrom(typeIndicator);
        var (body, footers) = BodyFrom(lines);
        typeIndicator = _customization.Sanitize(typeIndicator, footers);

        return new CommitMessage(typeIndicator, scope, description, body, footers);
    }

#if NET6_0
    private (string, string) HeaderFrom(string? header)
    {
        var twoParts = header?.Split(_customization.Separator);
        return twoParts?.Length == 2
            ? (twoParts.First().Trim(), twoParts.Last().Trim())
            : ("", "");
    }
#elif NET7_0_OR_GREATER
    private (string, string) HeaderFrom(string? header) =>
        header?.Split(_customization.Separator) is [var first, var second]
            ? (first.Trim(),second.Trim())
            : ("", "");
#endif

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
