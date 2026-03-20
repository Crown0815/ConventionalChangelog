using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_changelog_from_a_git_repository_configured_with_include_directories : GitUsingTestsBase
{
    [Fact]
    public void only_includes_commits_that_modified_files_in_the_given_include_directories()
    {
        Repository.Commit(Feature.CommitWith(A.Description(1)).Message, "folder1/file.txt");
        Repository.Commit(Feature.CommitWith(A.Description(2)).Message, "folder2/file.txt");
        Repository.Commit(Feature.CommitWith(A.Description(3)).Message, "other/file.txt");

        var configuration = new Configuration(includeDirectories: ["folder1", "folder2"]);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1), configuration);
    }

    [Fact]
    public void includes_all_commits_when_no_include_directories_are_configured()
    {
        Repository.Commit(Feature.CommitWith(A.Description(1)).Message, "folder1/file.txt");
        Repository.Commit(Feature.CommitWith(A.Description(2)).Message, "folder2/file.txt");

        var configuration = new Configuration(includeDirectories: []);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1), configuration);
    }
}
