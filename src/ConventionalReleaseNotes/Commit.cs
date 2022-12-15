namespace ConventionalReleaseNotes;

public record Commit(string Message, string? Hash = null)
{
    public string Hash { get; } = Hash ?? Guid.NewGuid().ToString();
}