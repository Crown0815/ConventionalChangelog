using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using LibGit2Sharp;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

internal static class RepositoryAssertionExtensions
{
    public static RepositoryChangelogAssertions Should(this Repository instance) => new(instance);

    public class RepositoryChangelogAssertions(Repository subject)
        : ReferenceTypeAssertions<Repository, RepositoryChangelogAssertions>(subject)
    {
        private const string ExpectedMatchingChangelogButDifferentWasFound = """
                    Expected {context:repo} to have changelog {0}, but found {1}.

                    GitGraph:
                    {2}
                    """;

        [ExcludeFromCodeCoverage]
        protected override string Identifier => "repo";

        public void HaveChangelogMatching(string changelog, IConfiguration? configuration = default)
        {
            var conventionalChangelog = new Changelog(configuration ?? new Configuration());
            Execute.Assertion
                .Given(() => conventionalChangelog.FromRepository(Subject.Path()))
                .ForCondition(c => c == changelog)
                .FailWith(ExpectedMatchingChangelogButDifferentWasFound,
                    _ => changelog, c => c, _ => Environment.NewLine + Subject.LogGraph());
        }
    }
}
