﻿using LibGit2Sharp;
using Xunit;
using static ConventionalChangelog.Unit.Tests.CommitTypeFor;

namespace ConventionalChangelog.Unit.Tests.Integration;

public class Git_specs : GitUsingTestsBase
{
    private static string Version(string version) => $"v{version}";

    [Fact]
    public void An_empty_repository_produces_an_empty_changelog()
    {
        Repository.Should().HaveChangelogMatching(A.Changelog.Empty);
    }

    [Fact]
    public void A_repository_with_conventional_commits_produces_changelog_with_all_conventional_commit_messages()
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
    public void Changelog_from_only_conventional_commits_contains_messages_by_default_from_newest_to_oldest_commit()
    {
        3.Times(i => Repository.Commit(Feature, i));
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Theory]
    [InlineData(ChangelogOrder.NewestToOldest, new[]{2,1,0})]
    [InlineData(ChangelogOrder.OldestToNewest, new[]{0,1,2})]
    public void Changelog_from_only_conventional_commits_contains_messages_in_requested_order(ChangelogOrder order, int[] commits)
    {
        3.Times(i => Repository.Commit(Feature, i));
        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, commits), order);
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
    public void Changelog_from_conventional_commits_and_a_single_tag_should_contain_all_commits_after_the_tag(string number)
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag(Version(number));

        3.Times(i => Repository.Commit(Feature, i));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_multiple_tags_should_contain_all_commits_after_the_last_tag()
    {
        Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Tagged commit").Tag(Version("1.0.0"));

        Repository.Commit(Feature, "Before tag 2");
        Repository.Commit(Feature, "Tagged commit 2").Tag(Version("2.0.0"));

        3.Times(i => Repository.Commit(Feature, i));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_non_version_tags_should_contain_all_commits()
    {
        Repository.Commit(Feature, 1).Tag("a");
        Repository.Commit(Feature, 2).Tag("b");

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1));
    }

    [Fact]
    public void Changelog_from_conventional_commits_and_a_multiple_tags_on_same_commit_contains_all_commits_after_the_tags()
    {
        Repository.Commit(Feature, "Before tags");
        var commit = Repository.Commit(Feature, "Multi-tagged commit");
        commit.Tag(Version("1.0.0"));
        commit.Tag(Version("1.0.1"));

        3.Times(i => Repository.Commit(Feature, i));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 2, 1, 0));
    }

    [Fact]
    public void Changelog_from_conventional_commits_with_fix_up_commits_excludes_those_fix_ups_with_their_target_in_the_changelog()
    {
        var after = Repository.Commit(Feature.CommitWithDescription(1));
        Repository.Commit(Feature.CommitWithDescription(2).WithFooter("Fixes", after.Sha));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 1));
    }

    [Fact]
    public void Changelog_from_conventional_commits_with_fix_up_commits_excludes_those_fix_ups_with_their_target_in_the_changelog2()
    {
        var before = Repository.Commit(Feature, "Before tag");
        Repository.Commit(Feature, "Multi-tagged commit").Tag(Version("1.0.0"));

        var after = Repository.Commit(Feature.CommitWithDescription(1));
        Repository.Commit(Feature.CommitWithDescription(2).WithFooter("Fixes", after.Sha));
        Repository.Commit(Feature.CommitWithDescription(3).WithFooter("Fixes", before.Sha));

        Repository.Should().HaveChangelogMatching(A.Changelog.WithGroup(Feature, 3, 1));
    }

    [Fact]
    public void Changelog_from_branched_conventional_commits_contains_messages_from_all_commits_since_last_tag_on_current_branch()
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
    public void Changelog_from_merged_conventional_commits_contains_messages_from_all_commits_since_last_tag_on_first_parent()
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
    public void Changelog_from_merged_conventional_commits_contains_messages_from_all_commits_since_last_tag_on_first_parent1()
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
