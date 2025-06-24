using System;

public static class MatchingEngineFactory
{
    public static IMatchingEngine Create(string strategy)
    {
        return strategy switch
        {
            "PriceTime" => new PriceTimePriorityMatchingEngine(),
            "ProRata" => new ProRataMatchingEngine(),
            _ => throw new ArgumentException("Unknown strategy")
        };
    }
}
