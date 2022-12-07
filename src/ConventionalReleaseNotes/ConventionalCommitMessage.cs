using System.Text.RegularExpressions;
using static ConventionalReleaseNotes.ConventionalCommitMessage;

namespace ConventionalReleaseNotes;

public record ConventionalCommitMessage(string Type, string Description, string Body, IReadOnlyCollection<Footer> Footers)
{
    private const string Separator = ": "; // see https://www.conventionalcommits.org/en/v1.0.0/#specification

    private static readonly ConventionalCommitMessage None = new("", "", "", Array.Empty<Footer>());

    public static ConventionalCommitMessage Parse(string rawMessage) => rawMessage switch
    {
        null => throw new ArgumentNullException(nameof(rawMessage)),
        "" => None,
        _ => Parsed(rawMessage),
    };

    private static ConventionalCommitMessage Parsed(string rawMessage)
    {
        using var lines = new StringReader(rawMessage);
        return Read(lines);
    }

    private static ConventionalCommitMessage Read(TextReader lines)
    {
        var (type, description) = HeaderFrom(lines.ReadLine()!);
        var (body, footers) = BodyFrom(lines);

        if (footers.Any(x => x.Token is "BREAKING CHANGE" or "BREAKING-CHANGE"))
            type = type.Replace("!", "");

        return new ConventionalCommitMessage(type, description, body, footers);
    }

    private static (string, string) HeaderFrom(string header) => header.Split(Separator) is [_, _] twoParts
        ? (twoParts.First(),twoParts.Last().Trim())
        : ("", "");

    private static IEnumerable<string> LinesFrom(TextReader reader)
    {
        while (reader.ReadLine() is {} line)
            yield return line;
    }

    private static IEnumerable<Footer> FootersFrom(IEnumerable<string> lines)
    {
        Footer? buffer = null;
        foreach (var line in lines)
        {
            if (buffer is not null && IsFooter(line))
            {
                yield return buffer with {Value = buffer.Value.Trim()};
                buffer = null;
            }
            if (buffer is null && string.IsNullOrWhiteSpace(line))
                continue;
            if (buffer is not null)
                buffer = buffer with { Value = buffer.Value + Environment.NewLine + line};
            else
                buffer = FooterFrom(line);
        }

        if (buffer is not null)
            yield return buffer with {Value = buffer.Value.Trim()};
    }

    private static (string, IReadOnlyCollection<Footer>) BodyFrom(TextReader reader)
    {
        var bodyParts = new List<string>();
        var footers = Enumerable.Empty<Footer>();
        while (reader.ReadLine() is { } line)
        {
            if (IsFooter(line))
            {
                footers = FootersFrom(LinesFrom(reader).Prepend(line));
                break;
            }
            bodyParts.Add(line);
        }

        return (string.Join(Environment.NewLine, bodyParts).Trim(), footers.ToList());
    }

    private static bool IsFooter(string line)
    {
        if (Regex.IsMatch(line, @"^#\w+-\d+")) return true;
        return Regex.IsMatch(line, @"^(?<token>[\w\-]+|BREAKING[ -]CHANGE)(?<separator>: | #)");
    }

    private static Footer FooterFrom(string line)
    {
        if (Regex.IsMatch(line, @"^#\w+-\d+"))
        {
            var x = line.Split(" ");
            var token1 = x.First().Replace("#", "");
            var value1 = line.Replace("#"+token1, "").Trim();
            return new Footer(token1, value1);
        }

        var match = Regex.Match(line, @"^(?<token>[\w\-]+|BREAKING[ -]CHANGE)(?<separator>: | #)");

        var token = match.Groups["token"].Value;
        var value = line.Replace(match.Value, "");
        return new Footer(token, value);
    }

    public record Footer(string Token, string Value);
}
