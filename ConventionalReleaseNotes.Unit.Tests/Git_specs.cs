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
    public void Changelog_from_an_empty_repository_should_be_empty()
    {
        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty);
    }

    [Fact]
    public void Changelog_from_only_conventional_commits_contains_the_messages_of_the_commits()
    {
        _repository.Commit(Feature, "new feature");
        _repository.Commit(Bugfix, "new fix");
        _repository.Commit(PerformanceImprovement, "new performance improvement");

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet("new feature")
            .WithGroup(Bugfix.Header)
                .WithBullet("new fix")
            .WithGroup(PerformanceImprovement.Header)
                .WithBullet("new performance improvement"));
    }

    [Fact]
    public void Changelog_from_only_conventional_commits_contains_messages_from_newest_to_oldest_commit()
    {
        3.Times(i => _repository.Commit(Feature, $"Commit {i}"));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet("Commit 2")
                .WithBullet("Commit 1")
                .WithBullet("Commit 0"));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_a_single_tag_should_contain_all_commits_after_the_tag()
    {
        _repository.Commit(Feature, "Before tag");
        var target = _repository.Commit(Feature, "Tagged commit");
        _repository.Tags.Add("v1.0.0", target);

        3.Times(i => _repository.Commit(Feature, $"After tag {i}"));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet("After tag 2")
                .WithBullet("After tag 1")
                .WithBullet("After tag 0"));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_multiple_tags_should_contain_all_commits_after_the_last_tag()
    {
        _repository.Commit(Feature, "Before tag 2");
        var target = _repository.Commit(Feature, "Tagged commit 1");
        _repository.Tags.Add("v1.0.0", target);

        _repository.Commit(Feature, "Before tag 2");
        target = _repository.Commit(Feature, "Tagged commit 2");
        _repository.Tags.Add("v2.0.0", target);

        3.Times(i => _repository.Commit(Feature, $"After tag {i}"));

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup(Feature.Header)
                .WithBullet("After tag 2")
                .WithBullet("After tag 1")
                .WithBullet("After tag 0"));
    }

    public void Dispose() => _repository.Delete();
}
