using System;

public static class MatchingEngineFactory
{
    public static IMatchingEngine Create(string strategy)
    {
        return Create(strategy, null);
    }

    public static IMatchingEngine Create(string strategy, ILogger? logger)
    {
        IMatchingEngine engine = strategy switch
        {
            "PriceTime" => new PriceTimePriorityMatchingEngine(logger),
            "ProRata" => new ProRataMatchingEngine(logger),
            _ => throw new ArgumentException("Unknown strategy")
        };

        return logger != null
            ? new LoggingMatchingEngineDecorator(engine, logger)
            : engine;
    }
}
