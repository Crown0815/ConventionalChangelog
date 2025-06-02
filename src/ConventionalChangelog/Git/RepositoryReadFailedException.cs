namespace ConventionalChangelog.Git;

public class RepositoryReadFailedException(string message, Exception inner) : Exception(message, inner);
