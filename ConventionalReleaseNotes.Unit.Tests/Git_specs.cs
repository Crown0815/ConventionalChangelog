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
    public void Changelog_from_a_single_branch_repository_with_conventional_commits_should_contain_all_commits()
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

    public void Dispose() => _repository.Delete();
}
