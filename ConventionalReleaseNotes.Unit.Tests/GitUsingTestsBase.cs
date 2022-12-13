using System;
using System.IO;
using LibGit2Sharp;

namespace ConventionalReleaseNotes.Unit.Tests;

public abstract class GitUsingTestsBase : IDisposable
{
    protected readonly Repository Repository;

    protected GitUsingTestsBase()
    {
        var path = Path.Combine(Path.GetTempPath(), TestDirectoryName);
        Repository = new Repository(Repository.Init(path));
    }

    private const string TestDirectoryName = "unittest";
    public void Dispose() => Repository.Delete();
}
