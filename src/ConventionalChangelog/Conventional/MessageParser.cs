﻿using static ConventionalChangelog.Conventional.CommitMessage;

namespace ConventionalChangelog.Conventional;

public class MessageParser
{
    private readonly IConfiguration _configuration;

    public MessageParser(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CommitMessage Parse(string rawMessage)
    {
        using var lines = new StringReader(rawMessage);
        return Read(lines);
    }

    private CommitMessage Read(TextReader lines)
    {
        var (typeIndicator, description) = HeaderFrom(lines.ReadLine());
        var (body, footers) = BodyFrom(lines);
        typeIndicator = _configuration.Sanitize(typeIndicator, footers);

        return new CommitMessage(typeIndicator, description, body, footers);
    }

#if NET6_0
    private (string, string) HeaderFrom(string? header)
    {
        var twoParts = header?.Split(_configuration.Separator);
        return twoParts?.Length == 2
            ? (twoParts.First(), twoParts.Last().Trim())
            : ("", "");
    }
#elif NET7_0_OR_GREATER
    private (string, string) HeaderFrom(string? header) => header?.Split(_configuration.Separator) is [var first, var second]
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
