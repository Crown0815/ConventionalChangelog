using System;

namespace ConventionalChangelog.Unit.Tests;

public static class ActionRepetitionExtensions
{
    public static void Times(this int count, Action<int> action)
    {
        for (var i = 0; i < count; i++) action(i);
    }
}