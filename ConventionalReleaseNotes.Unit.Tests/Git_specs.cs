using System;
using System.IO;
using FluentAssertions;
using LibGit2Sharp;
using Xunit;
using static ConventionalReleaseNotes.Unit.Tests.CommitType;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Git_specs : IDisposable
{
    private const string TestDirectoryName = "unittest";

    private readonly Repository _repository;

    public Git_specs()
    {
        var path = Path.Combine(Path.GetTempPath(), TestDirectoryName);
        _repository = new Repository(Repository.Init(path));
    }

    [Fact]
    public void An_empty_repository_produces_an_empty_changelog()
    {
        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty);
    }

    [Fact]
    public void A_repository_with_conventional_commits_produces_changelog_with_all_conventional_commit_messages()
    {
        _repository.Commit("non conventional commit message should not appear in changelog");
        _repository.Commit(Feature, Model.Message(1));
        _repository.Commit(Bugfix, Model.Message(2));
        _repository.Commit(PerformanceImprovement, Model.Message(3));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(1))
            .WithGroup(Bugfix.Header)
                .WithBullet(Model.Message(2))
            .WithGroup(PerformanceImprovement.Header)
                .WithBullet(Model.Message(3)));
    }

    [Fact]
    public void Changelog_from_only_conventional_commits_contains_messages_from_newest_to_oldest_commit()
    {
        3.Times(i => _repository.Commit(Feature, Model.Message(i)));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(2))
                .WithBullet(Model.Message(1))
                .WithBullet(Model.Message(0)));
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
        _repository.Commit(Feature, "Before tag");
        _repository.Commit(Feature, "Tagged commit").Tag($"v{version}");

        3.Times(i => _repository.Commit(Feature, Model.Message(i)));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(2))
                .WithBullet(Model.Message(1))
                .WithBullet(Model.Message(0)));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_multiple_tags_should_contain_all_commits_after_the_last_tag()
    {
        _repository.Commit(Feature, "Before tag");
        _repository.Commit(Feature, "Tagged commit").Tag("v1.0.0");

        _repository.Commit(Feature, "Before tag 2");
        _repository.Commit(Feature, "Tagged commit 2").Tag("v2.0.0");

        3.Times(i => _repository.Commit(Feature, Model.Message(i)));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(2))
                .WithBullet(Model.Message(1))
                .WithBullet(Model.Message(0)));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_non_version_tags_should_contain_all_commits()
    {
        _repository.Commit(Feature, Model.Message(1)).Tag("a");
        _repository.Commit(Feature, Model.Message(2)).Tag("b");

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet(Model.Message(2))
                .WithBullet(Model.Message(1)));
    }

    public void Dispose() => _repository.Delete();
}
