using static System.Array;
using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

internal class MessageParser2
{
    private const string Separator = ": "; // see https://www.conventionalcommits.org/en/v1.0.0/#specification
    private static readonly CommitMessage None = new("", "", "", Empty<Footer>());
    private readonly IConfiguration _configuration;

    public MessageParser2(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CommitMessage Parse(string rawMessage) => rawMessage switch
    {
        null => throw new ArgumentNullException(nameof(rawMessage)),
        "" => None,
        _ => InnerParse(rawMessage),
    };

    private CommitMessage InnerParse(string rawMessage)
    {
        using var lines = new StringReader(rawMessage);
        return Read(lines);
    }

    private CommitMessage Read(TextReader lines)
    {
        var (typeIndicator, description) = HeaderFrom(lines.ReadLine()!);
        var (body, footers) = BodyFrom(lines);
        typeIndicator = _configuration.TypeFor(typeIndicator, footers);

        return new CommitMessage(typeIndicator, description, body, footers);
    }

#if NET6_0
    private static (string, string) HeaderFrom(string header)
    {
        var twoParts = header.Split(Separator);
        return twoParts.Length == 2
            ? (twoParts.First(), twoParts.Last().Trim())
            : ("", "");
    }
#elif NET7_0_OR_GREATER
    private static (string, string) HeaderFrom(string header) => header.Split(Separator) is [var first, var second]
        ? (first,second.Trim())
        : ("", "");
#endif

    private (string, IReadOnlyCollection<Footer>) BodyFrom(TextReader reader)
    {
        var bodyParts = new List<string>();
        var footers = Enumerable.Empty<Footer>();
        while (reader.ReadLine() is { } line)
        {
            if (_configuration.IsFooter(line))
            {
                footers = FootersFrom(LinesFrom(reader).Prepend(line));
                break;
            }
            bodyParts.Add(line);
        }

        return (string.Join(Environment.NewLine, bodyParts).Trim(), footers.ToList());
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
            if (buffer is not null && _configuration.IsFooter(line))
            {
                yield return buffer with {Value = buffer.Value.Trim()};
                buffer = null;
            }
            if (buffer is not null)
                buffer = buffer with { Value = buffer.Value + Environment.NewLine + line};
            else
                buffer = _configuration.FooterFrom(line);
        }

        if (buffer is not null)
            yield return buffer with {Value = buffer.Value.Trim()};
    }
}

internal static class MessageParser
{
    public static CommitMessage Parse(string rawMessage, IConfiguration configuration) => new MessageParser2(configuration).Parse(rawMessage);
}
