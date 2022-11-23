using System;
using System.IO;
using FluentAssertions;
using LibGit2Sharp;
using Xunit;

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

    public void Dispose() => _repository.Delete();
}
