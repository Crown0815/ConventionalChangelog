using static System.Array;
using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

internal static class MessageParser
{
    private const string Separator = ": "; // see https://www.conventionalcommits.org/en/v1.0.0/#specification

    private static readonly CommitMessage None = new(CommitType.None, "", "", Empty<Footer>());

    public static CommitMessage Parse(string rawMessage, IConfiguration configuration) => rawMessage switch
    {
        null => throw new ArgumentNullException(nameof(rawMessage)),
        "" => None,
        _ => Parsed(rawMessage, configuration),
    };

    private static CommitMessage Parsed(string rawMessage, IConfiguration configuration)
    {
        using var lines = new StringReader(rawMessage);
        return Read(lines, configuration);
    }

    private static CommitMessage Read(TextReader lines, IConfiguration configuration)
    {
        var (typeIndicator, description) = HeaderFrom(lines.ReadLine()!);
        var (body, footers) = BodyFrom(lines, configuration);

        if (footers.Any(x => x.IsBreakingChange))
            typeIndicator = typeIndicator.Replace(BreakingChange.Indicator, "");

        var type = configuration.TypeFor(typeIndicator);

        return new CommitMessage(type, description, body, footers);
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

    private static IEnumerable<string> LinesFrom(TextReader reader)
    {
        while (reader.ReadLine() is {} line)
            yield return line;
    }

    private static IEnumerable<Footer> FootersFrom(IEnumerable<string> lines, IConfiguration configuration)
    {
        Footer? buffer = null;
        foreach (var line in lines)
        {
            if (buffer is not null && configuration.IsFooter(line))
            {
                yield return buffer with {Value = buffer.Value.Trim()};
                buffer = null;
            }
            if (buffer is not null)
                buffer = buffer with { Value = buffer.Value + Environment.NewLine + line};
            else
                buffer = configuration.FooterFrom(line);
        }

        if (buffer is not null)
            yield return buffer with {Value = buffer.Value.Trim()};
    }

    private static (string, IReadOnlyCollection<Footer>) BodyFrom(TextReader reader, IConfiguration configuration)
    {
        var bodyParts = new List<string>();
        var footers = Enumerable.Empty<Footer>();
        while (reader.ReadLine() is { } line)
        {
            if (configuration.IsFooter(line))
            {
                footers = FootersFrom(LinesFrom(reader).Prepend(line), configuration);
                break;
            }
            bodyParts.Add(line);
        }

        return (string.Join(Environment.NewLine, bodyParts).Trim(), footers.ToList());
    }
}
