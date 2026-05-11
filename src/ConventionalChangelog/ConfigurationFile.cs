using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ConventionalChangelog;

internal static class ConfigurationFile
{
    public static ConfigurationValues Read(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        var content = File.ReadAllText(path);
        var configuration = deserializer.Deserialize<YamlConfiguration>(content) ?? new YamlConfiguration();

        return new ConfigurationValues(
            CommitTypes: configuration.CommitTypes?
                .Select(type => new CommitType(type.TypeIndicator, type.GroupHeader, ParseRelevance(type.Relevance)))
                .ToArray(),
            Scopes: configuration.Scopes?
                .Select(scope => new Scope(scope.Name, scope.Header))
                .ToArray());
    }

    private static Relevance ParseRelevance(string value)
    {
        return Enum.TryParse<Relevance>(value, true, out var relevance)
            ? relevance
            : Relevance.Ignore;
    }

    public readonly record struct ConfigurationValues(
        IReadOnlyCollection<CommitType>? CommitTypes,
        IReadOnlyCollection<Scope>? Scopes);

    private sealed class YamlConfiguration
    {
        public YamlCommitType[]? CommitTypes { get; init; }
        public YamlScope[]? Scopes { get; init; }
    }

    private sealed class YamlCommitType
    {
        public string TypeIndicator { get; init; } = "";
        public string GroupHeader { get; init; } = "";
        public string Relevance { get; init; } = "Ignore";
    }

    private sealed class YamlScope
    {
        public string Name { get; init; } = "";
        public string? Header { get; init; }
    }
}
