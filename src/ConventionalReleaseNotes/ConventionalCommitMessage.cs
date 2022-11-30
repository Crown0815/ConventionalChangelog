using System.Text.RegularExpressions;

namespace ConventionalReleaseNotes;

public record ConventionalCommitMessage(string Type, string Description, string Body, IReadOnlyCollection<ConventionalCommitFooter> Footers)
{
    public static ConventionalCommitMessage Parse(string rawMessage)
    {
        using var reader = new StringReader(rawMessage);
        var header = reader.ReadLine()!;
        var parts = header.Split(": ");
        var type = parts.First();
        var description = header.Replace(type+": ", "");

        var (body, footers) = BodyFrom(reader);

        return new ConventionalCommitMessage(type, description.Trim(), body, footers.ToList());
    }

    private static IEnumerable<string> LinesFrom(TextReader reader)
    {
        while (reader.ReadLine() is {} line)
            yield return line;
    }

    private static IEnumerable<ConventionalCommitFooter> FootersFrom(IEnumerable<string> lines)
    {
        ConventionalCommitFooter? buffer = null;
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

    private static (string, IEnumerable<ConventionalCommitFooter>) BodyFrom(TextReader reader)
    {
        var bodyParts = new List<string>();
        var footers = Enumerable.Empty<ConventionalCommitFooter>();
        while (reader.ReadLine() is { } line)
        {
            if (IsFooter(line))
            {
                footers = FootersFrom(LinesFrom(reader).Prepend(line));
                break;
            }
            bodyParts.Add(line);
        }

        return (string.Join(Environment.NewLine, bodyParts).Trim(), footers);
    }

    private static bool IsFooter(string line)
    {
        if (Regex.IsMatch(line, @"^#\w+-\d+")) return true;
        return Regex.IsMatch(line, @"^(?<token>[\w\-]+|BREAKING[ -]CHANGE)(?<separator>: | #)");
    }

    private static ConventionalCommitFooter FooterFrom(string line)
    {
        if (Regex.IsMatch(line, @"^#\w+-\d+"))
        {
            var x = line.Split(" ");
            var token1 = x.First().Replace("#", "");
            var value1 = line.Replace("#"+token1, "").Trim();
            return new ConventionalCommitFooter(token1, value1);
        }

        var match = Regex.Match(line, @"^(?<token>[\w\-]+|BREAKING[ -]CHANGE)(?<separator>: | #)");

        var token = match.Groups["token"].Value;
        var value = line.Replace(match.Value, "");
        return new ConventionalCommitFooter(token, value);
    }

}

public record ConventionalCommitFooter(string Token, string Value);
