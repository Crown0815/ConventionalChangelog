using System.IO;
using LibGit2Sharp;

namespace ConventionalReleaseNotes.Unit.Tests;

internal static class RepositoryExtensions
{
    public static void Delete(this Repository repository)
    {
        repository.Dispose();
        ForceDelete(new DirectoryInfo(repository.Path()));
    }

    public static string Path(this Repository r) => r.Info.Path;

    private static void ForceDelete(DirectoryInfo directory)
    {
        directory.RemoveReadOnlyAttributeFromFiles();
        directory.Delete(true);
    }

    private static void RemoveReadOnlyAttributeFromFiles(this DirectoryInfo directory)
    {
        foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            info.Attributes = FileAttributes.Normal;
    }
}