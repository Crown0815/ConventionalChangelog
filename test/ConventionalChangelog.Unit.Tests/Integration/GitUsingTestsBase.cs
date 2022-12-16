using System;
using System.IO;
using LibGit2Sharp;

namespace ConventionalChangelog.Unit.Tests.Integration;

public abstract class GitUsingTestsBase : IDisposable
{
    protected readonly Repository Repository;

    protected GitUsingTestsBase()
    {
        var path = Path.Combine(Path.GetTempPath(), GetType().FullName!);
        Repository = new Repository(Repository.Init(path));
    }

    public void Dispose()
    {
        Repository.Dispose();
        ForceDelete(new DirectoryInfo(Repository.Path()));
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
