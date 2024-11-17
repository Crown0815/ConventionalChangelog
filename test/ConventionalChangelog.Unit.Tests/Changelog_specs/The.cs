namespace ConventionalChangelog.Unit.Tests.Changelog_specs;

internal static class The
{
    private static readonly Changelog ConventionalChangelog = new(new Configuration());

    public static string ChangelogFrom(params Commit[] messages) => ConventionalChangelog.From(messages);

    public static Changelog ChangelogWith(Configuration config) => new(config);
}
