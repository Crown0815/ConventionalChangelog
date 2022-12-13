using FluentAssertions;
using Xunit;
using static ConventionalReleaseNotes.Unit.Tests.CommitTypeFor;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Git_specs : GitUsingTestsBase
{
    [Fact]
    public void An_empty_repository_produces_an_empty_changelog()
    {
        Changelog.FromRepository(Repository.Path()).Should().Be(Model.Changelog.Empty);
    }

    [Fact]
    public void A_repository_with_conventional_commits_produces_changelog_with_all_conventional_commit_messages()
    {
        Repository.Commit("non conventional commit message should not appear in changelog");
        Repository.Commit(Feature, Model.Description(1));
        Repository.Commit(Bugfix, Model.Description(2));
        Repository.Commit(PerformanceImprovement, Model.Description(3));

        Changelog.FromRepository(Repository.Path())
            .Should().Be(Model.Changelog.Empty
                .WithGroup(Feature, 1)
                .WithGroup(Bugfix, 2)
                .WithGroup(PerformanceImprovement, 3));
    }

    [Fact]
    public void Changelog_from_only_conventional_commits_contains_messages_from_newest_to_oldest_commit()
    {
        3.Times(i => Repository.Commit(Feature, Model.Description(i)));

        Changelog.FromRepository(Repository.Path())
            .Should().Be(Model.Changelog.Empty
                .WithGroup(Feature, 2, 1, 0));
    }

    [Theory]
    [InlineData("0.0.4")]
    [InlineData("1.2.3")]
    [InlineData("10.20.30")]
    [InlineData("1.1.2-prerelease+meta")]
    [InlineData("1.1.2+meta")]
    [InlineData("1.0.0-alpha")]
    [InlineData("1.0.0-beta")]
    [InlineData("1.0.0-alpha.1")]
    public void Changelog_from_conventional_commits_and_a_single_tag_should_contain_all_commits_after_the_tag(string version)
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag($"v{version}");

        3.Times(i => Repository.Commit(Feature, Model.Description(i)));

        Changelog.FromRepository(Repository.Path())
            .Should().Be(Model.Changelog.Empty
                .WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_multiple_tags_should_contain_all_commits_after_the_last_tag()
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag("v1.0.0");

        Repository.Commit(Feature, "Before tag 2");
        Repository.Commit(Feature, "Tagged commit 2").Tag("v2.0.0");

        3.Times(i => Repository.Commit(Feature, Model.Description(i)));

        Changelog.FromRepository(Repository.Path())
            .Should().Be(Model.Changelog.Empty
                .WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_non_version_tags_should_contain_all_commits()
    {
        Repository.Commit(Feature, Model.Description(1)).Tag("a");
        Repository.Commit(Feature, Model.Description(2)).Tag("b");

        Changelog.FromRepository(Repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature, 2, 1));
    }
}
