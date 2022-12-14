using System;
using System.IO;
using LibGit2Sharp;

namespace ConventionalReleaseNotes.Unit.Tests.Integration;

public abstract class GitUsingTestsBase : IDisposable
{
    protected readonly Repository Repository;

    protected GitUsingTestsBase()
    {
        var path = Path.Combine(Path.GetTempPath(), GetType().FullName!);
        Repository = new Repository(Repository.Init(path));
    }

    public void Dispose() => Repository.Delete();
}
