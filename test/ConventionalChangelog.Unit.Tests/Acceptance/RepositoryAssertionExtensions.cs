﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using LibGit2Sharp;

namespace ConventionalChangelog.Unit.Tests.Acceptance;

internal static class RepositoryAssertionExtensions
{
    public static RepositoryChangelogAssertions Should(this Repository instance) => new(instance);

    public class RepositoryChangelogAssertions : ReferenceTypeAssertions<Repository, RepositoryChangelogAssertions>
    {
#if NET6_0
        private const string ExpectedMatchingChangelogButDifferentWasFound = "Expected {context:repo} to have changelog {0}, but found {1}.";
#elif NET7_0_OR_GREATER
        private const string ExpectedMatchingChangelogButDifferentWasFound = """
                    Expected {context:repo} to have changelog {0}, but found {1}.

                    GitGraph:
                    {2}
                    """;
#endif

        public RepositoryChangelogAssertions(Repository subject) : base(subject)
        {
        }

        [ExcludeFromCodeCoverage]
        protected override string Identifier => "repo";

        public void HaveChangelogMatching(string changelog, IConfiguration? configuration = default)
        {
            var conventionalChangelog = new Changelog(configuration ?? new Configuration.Configuration());
            Execute.Assertion
                .Given(() => conventionalChangelog.FromRepository(Subject.Path()))
                .ForCondition(c => c == changelog)
                .FailWith(ExpectedMatchingChangelogButDifferentWasFound,
                    _ => changelog, c => c, _ => Environment.NewLine + LogGraphOf(Subject));
        }

        private static string LogGraphOf(Repository r)
        {
            using var process = new Process();

            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = $"-C {r.Path()} log --graph --all --oneline --decorate";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
    }
}
