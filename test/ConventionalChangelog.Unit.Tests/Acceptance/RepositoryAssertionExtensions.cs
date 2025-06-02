using System;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;
using LibGit2Sharp;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

internal static class RepositoryAssertionExtensions
{
    public static RepositoryChangelogAssertions Should(this Repository instance)
    {
        return new RepositoryChangelogAssertions(instance, AssertionChain.GetOrCreate());
    }

    public class RepositoryChangelogAssertions(Repository subject, AssertionChain chain)
        : ReferenceTypeAssertions<Repository, RepositoryChangelogAssertions>(subject, chain)
    {
        private readonly AssertionChain _chain = chain;

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
            _chain
                .Given(() => conventionalChangelog.FromRepository(Subject.Path()))
                .ForCondition(c => c == changelog)
                .FailWith(ExpectedMatchingChangelogButDifferentWasFound,
                    _ => changelog, c => c, _ => Environment.NewLine + Subject.LogGraph());
        }

        public void ThrowWhenCreatingChangelog<T>(IConfiguration? configuration = default) where T : Exception
        {
            var conventionalChangelog = new Changelog(configuration ?? new Configuration());
            var creatingChangelog = () => conventionalChangelog.FromRepository(Subject.Path());
            creatingChangelog.Should().Throw<T>();
        }
    }
}
