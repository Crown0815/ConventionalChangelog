using FluentAssertions;
using Xunit;

namespace ConventionalReleaseNotes.Unit.Tests.Integration;

public class Helios_repository_specs : GitUsingTestsBase
{
    [Fact]
    public void Changelog_from_repository_supports_prerelease_version_tags_indicated_by_p()
    {
        Repository.Commit(CommitTypeFor.Feature, "Before tag");
        Repository.Commit(CommitTypeFor.Feature, "Tagged commit").Tag("p1.0.0-alpha.1");

        3.Times(i => Repository.CommitWithDescription(CommitTypeFor.Feature, i));

        Changelog.FromRepository(Repository.Path())
            .Should().Be(A.Changelog.WithGroup(CommitTypeFor.Feature, 2, 1, 0));
    }
}
