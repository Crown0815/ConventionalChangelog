using System;
using System.IO;
using LibGit2Sharp;

namespace ConventionalChangelog.Unit.Tests.Integration;

public abstract class GitUsingTestsBase : IDisposable
{
    protected readonly Repository Repository;
    private readonly string _randomDirectoryName = Guid.NewGuid().ToString();

    protected GitUsingTestsBase()
    {
        var path = Path.Combine(Path.GetTempPath(), _randomDirectoryName);
        Repository = new Repository(Repository.Init(path));
    }

    public virtual void Dispose()
    {
        Repository.Dispose();
        ForceDelete(new DirectoryInfo(Repository.Path()));
        GC.SuppressFinalize(this);
    }

    private static void ForceDelete(DirectoryInfo directory)
    {
        RemoveReadOnlyAttributeFromFilesIn(directory);
        directory.Delete(true);
    }

    private static void RemoveReadOnlyAttributeFromFilesIn(DirectoryInfo directory)
    {
        foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            info.Attributes = FileAttributes.Normal;
    }
}
