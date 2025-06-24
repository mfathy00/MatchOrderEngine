using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class StressTests
{
    [Fact]
    public async Task PriceTimePriorityEngine_HandlesLargeOrderBook()
    {
        var orders = new List<Order>();
        var now = DateTime.UtcNow;
        for (int i = 0; i < 50000; i++)
        {
            orders.Add(new Order("A", $"B{i}", OrderDirection.Buy, 1, 5.00m, now.AddSeconds(i)));
        }
        for (int i = 0; i < 50000; i++)
        {
            orders.Add(new Order("A", $"S{i}", OrderDirection.Sell, 1, 5.00m, now.AddSeconds(50000 + i)));
        }

        var engine = new PriceTimePriorityMatchingEngine();
        var results = await engine.MatchAsync(orders).ConfigureAwait(false);

        Assert.Equal(100000, results.Count());
    }
}
