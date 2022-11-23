using System;
using System.IO;
using FluentAssertions;
using LibGit2Sharp;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests;

public class Git_specs : IDisposable
{
    private const string TestDirectoryName = "unittest";


    private static readonly Identity TestIdentity = new("unit test", "unit@test.email");
    private static readonly Signature Signature = new(TestIdentity, DateTimeOffset.Now);

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
        _repository.Commit(Model.ConventionalCommitMessage("feat", "new feature"), Signature, Signature, new CommitOptions
        {
            AllowEmptyCommit = true,
        });
        _repository.Commit(Model.ConventionalCommitMessage("fix", "new fix"), Signature, Signature, new CommitOptions
        {
            AllowEmptyCommit = true,
        });
        _repository.Commit(Model.ConventionalCommitMessage("perf", "new performance improvement"), Signature, Signature, new CommitOptions
        {
            AllowEmptyCommit = true,
        });

        Changelog.FromRepository(_repository.Path()).Should().Be(Model.Changelog.Empty
            .WithGroup("Features")
                .WithBullet("new feature")
            .WithGroup("Bug Fixes")
                .WithBullet("new fix")
            .WithGroup("Performance Improvements")
                .WithBullet("new performance improvement"));
    }

    public void Dispose() => _repository.Delete();
}
