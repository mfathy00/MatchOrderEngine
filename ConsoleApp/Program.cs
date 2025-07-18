﻿using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ConsoleApp <PriceTime|ProRata> <csvPath>");
            return 1;
        }

        var strategy = args[0];
        var path = args[1];

        var logger = new CsvLogger("logs.csv");
        LoggingSetup.RegisterGlobalExceptionHandlers(logger);

        var engine = MatchingEngineFactory.Create(strategy, logger);
        var orders = await logger.LogExecutionAsync("LoadOrders", () => OrderBookLoader.LoadFromCsvAsync(path));
        var results = await engine.MatchAsync(orders);

        foreach (var result in results)
        {
            Console.WriteLine($"{result.OrderId}: {result.MatchState}, Remaining Volume: {result.Volume}");
        }

        return 0;
    }
}
