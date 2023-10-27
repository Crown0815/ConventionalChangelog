using LibGit2Sharp;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Integration;

public class The_changelog_from_a_git_repository_using_conventional_commits : GitUsingTestsBase
{
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
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, expected), order);
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
        Repository.CreateBranch("develop");
        Repository.Checkout("develop");
        Repository.Commit(Feature, 3).Tag(Version("0.9.0-alpha.1"));
        Repository.Commit(Feature, 4);
        var end = Repository.Commit(Feature, 5);

        Repository.Checkout(root);
        Repository.CreateBranch("release/1.0.0", root);
        Repository.Checkout("release/1.0.0");
        Repository.Commit(Feature, 1).Tag(Version("1.0.0-beta.1"));
        Repository.Commit(Feature, 2);

        Repository.Checkout(end);

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
        Repository.Checkout("master");
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
        Repository.Checkout("master");
        Repository.Merge(develop).Tag(Version("0.1.0"));

        Repository.Checkout("develop");
        Repository.Commit(Feature, 3).Tag(Version("0.2.0-alpha.1"));
        Repository.Commit(Feature, 4).Tag(Version("0.2.0-alpha.2"));
        Repository.Checkout("master");
        Repository.Merge(develop);

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 4, 3));
    }
}
