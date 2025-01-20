namespace ConventionalChangelog;

public record Scope(string Indicator, string? GroupHeader)
{
    public static Scope None { get; } = new("", null);
}
