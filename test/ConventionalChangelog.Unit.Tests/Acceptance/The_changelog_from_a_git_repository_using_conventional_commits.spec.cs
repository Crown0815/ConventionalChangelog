using System.IO;
using ConventionalChangelog.Git;
using LibGit2Sharp;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

public class The_changelog_from_a_git_repository_using_conventional_commits : GitUsingTestsBase
{
    private const string DefaultBranch = "master";
    private static string Version(string version) => $"v{version}";

    [Fact]
    public void is_empty_if_the_repository_is_empty()
    {
        Repository.Should().HaveChangelogMatching(A.Changelog.Empty);
    }

    [Fact]
    public void contains_all_conventional_commit_messages()
    {
        Repository.Commit("non conventional commit message should not appear in changelog");
        Repository.Commit(Feature, 1);
        Repository.Commit(Bugfix, 2);
        Repository.Commit(PerformanceImprovement, 3);

        Repository.Should().HaveChangelogMatching(A.Changelog
            .WithGroup(Feature, 1)
            .And(Bugfix, 2)
            .And(PerformanceImprovement, 3));
    }

    [Fact]
    public void by_default_orders_the_messages_from_newest_to_oldest_commit()
    {
        3.Times(i => Repository.Commit(Feature, i));
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Theory]
    [InlineData(ChangelogOrder.NewestToOldest, new[]{2,1,0})]
    [InlineData(ChangelogOrder.OldestToNewest, new[]{0,1,2})]
    public void when_requested_orders_the_messages_from(ChangelogOrder order, int[] expected)
    {
        3.Times(i => Repository.Commit(Feature, i));
        var config = new Configuration { ChangelogOrder = order };
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, expected), config);
    }

    [Theory]
    [InlineData("0.0.4")]
    [InlineData("1.2.3")]
    [InlineData("10.20.30")]
    [InlineData("1.1.2-prerelease+meta")]
    [InlineData("1.1.2+meta")]
    [InlineData("1.0.0-alpha")]
    [InlineData("1.0.0-beta")]
    [InlineData("1.0.0-alpha.1")]
    public void and_containing_a_version_tag_contains_all_commits_after_the_tag(string number)
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag(Version(number));

        3.Times(i => Repository.Commit(Feature, i));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void and_containing_multiple_version_tags_produces_changelog_contain_all_commits_after_the_last_version_tag()
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag(Version("1.0.0"));

        Repository.Commit(Feature, "Before tag 2");
        Repository.Commit(Feature, "Tagged commit 2").Tag(Version("2.0.0"));

        3.Times(i => Repository.Commit(Feature, i));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    [InlineData("x1.0.0")]
    [InlineData("pv1.0.0")]
    [InlineData("vv1.0.0")]
    [InlineData("v1.0.0.0")]
    public void and_containing_non_version_tags_should_contain_all_commits(string nonVersionTag)
    {
        Repository.Commit(Feature, 1).Tag(nonVersionTag);
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 1));
    }

    [Fact]
    public void and_multiple_version_tags_on_the_same_commit_contains_all_commits_after_the_tags()
    {
        Repository.Commit(Feature, "Before tags");
        var commit = Repository.Commit(Feature, "Multi-tagged commit");
        commit.Tag(Version("1.0.0"));
        commit.Tag(Version("1.0.1"));

        3.Times(i => Repository.Commit(Feature, i));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void only_considers_commits_from_the_currently_checked_out_branch()
    {
        var root = Repository.Commit(Irrelevant, "Initial Commit");
        var develop = Repository.CreateAndCheckoutBranch("develop");
        Repository.Commit(Feature, 3).Tag(Version("0.9.0-alpha.1"));
        Repository.Commit(Feature, 4);
        Repository.Commit(Feature, 5);

        Repository.CreateBranch("release/1.0.0", root);
        Repository.Checkout("release/1.0.0");
        Repository.Commit(Feature, 1).Tag(Version("1.0.0-beta.1"));
        Repository.Commit(Feature, 2);

        Repository.Checkout(develop);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 5, 4));
    }

    [Fact]
    public void when_encountering_merge_commits_ignores_version_tags_on_merged_branches()
    {
        Repository.Commit(Irrelevant, "Initial Commit");
        var develop = Repository.CreateBranch("develop");
        Repository.Checkout("develop");
        Repository.Commit(Feature, 1).Tag(Version("v0.1.0-alpha.1"));
        Repository.Commit(Feature, 2).Tag(Version("0.1.0-alpha.2"));
        Repository.Checkout(DefaultBranch);
        Repository.Merge(develop);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1));
    }

    [Fact]
    public void when_encountering_merge_commits_considers_version_tags_on_merged_commits()
    {
        Repository.Commit(Irrelevant, "Initial Commit");
        var develop = Repository.CreateBranch("develop");
        Repository.Checkout("develop");
        Repository.Commit(Feature, 1).Tag(Version("0.1.0-alpha.1"));
        Repository.Commit(Feature, 2).Tag(Version("0.1.0-alpha.2"));
        Repository.Checkout(DefaultBranch);
        Repository.Merge(develop).Tag(Version("0.1.0"));

        Repository.Checkout("develop");
        Repository.Commit(Feature, 3).Tag(Version("0.2.0-alpha.1"));
        Repository.Commit(Feature, 4).Tag(Version("0.2.0-alpha.2"));
        Repository.Checkout(DefaultBranch);
        Repository.Merge(develop);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 4, 3));
    }

    [Fact]
    public void when_configured_to_ignore_pre_releases_then_only_non_release_version_tags_are_considered()
    {
        Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2).Tag(Version("0.1.0"));
        Repository.Commit(Feature, 3).Tag(Version("0.2.0-alpha.1"));
        Repository.Commit(Feature, 4);

        var config = new Configuration(ignorePrerelease: true);
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 4, 3), config);
    }

    [Fact]
    public void when_configured_to_skip_title_then_title_and_blank_line_are_excluded()
    {
        Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2);

        var config = new Configuration(skipTitle: true);
        Repository.Should().HaveChangelogMatching(A.Changelog.WithoutTitle().And(Feature, 2, 1), config);
    }

    [Fact]
    public void when_the_repository_contains_a_file_overwriting_a_commit_message_the_overwriting_message_is_printed()
    {
        Repository.Commit(Irrelevant, "Initial Commit");
        var commit = Repository.Commit(Feature, 1);

        var file = OverwriteMessageWithFile(commit, Feature.CommitWithDescription(3));
        Commands.Stage(Repository, file);
        Repository.Commit(Feature, 2);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 3));
        return;

        string OverwriteMessageWithFile(GitObject aCommit, Commit newCommit)
        {
            var directory = Path.Combine(Repository.Path(), ".conventional-changelog");
            Directory.CreateDirectory(directory);
            var aFile = Path.Combine(directory, $"{aCommit.Sha}");
            File.WriteAllText(aFile, newCommit.Message);
            return aFile;
        }
    }

    [Fact]
    public void when_configured_with_an_upstream_reference_commit_then_all_commits_after_the_reference_commit_are_considered()
    {
        var referenceCommit = Repository.Commit(Feature, 1);
        Repository.Commit(Feature, 2).Tag(Version("0.1.0"));
        Repository.Commit(Feature, 3);

        var config = new Configuration(referenceCommit: referenceCommit.Sha);
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 3, 2), config);
    }

    [Fact]
    public void when_configured_with_a_custom_non_existing_reference_commit_then_an_exception_is_thrown()
    {
        Repository.Commit(Feature, 1).Tag(Version("0.1.0"));
        Repository.Commit(Feature, 2);

        const string randomCommitSha = "0000000000000000000000000000000000000000";

        var config = new Configuration(referenceCommit: randomCommitSha);
        Repository.Should().ThrowWhenCreatingChangelog<RepositoryReadFailedException>(config);
    }
}
