using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class PriceTimePriorityMatchingEngineTests
{
    [Fact]
    public async Task MatchesAccordingToPriceTimePriority()
    {
        var orders = new[]
        {
            new Order("A", "A1", OrderDirection.Buy, 100, 4.99m, DateTime.Parse("09:27:43")),
            new Order("B", "B1", OrderDirection.Buy, 200, 5.00m, DateTime.Parse("10:21:46")),
            new Order("C", "C1", OrderDirection.Buy, 150, 5.00m, DateTime.Parse("10:26:18")),
            new Order("D", "D1", OrderDirection.Sell, 150, 5.00m, DateTime.Parse("10:32:41")),
            new Order("E", "E1", OrderDirection.Sell, 100, 5.00m, DateTime.Parse("10:33:07")),
        };

        var engine = new PriceTimePriorityMatchingEngine();
        var results = (await engine.MatchAsync(orders).ConfigureAwait(false)).ToDictionary(r => r.OrderId);

        Assert.Equal(MatchState.NoMatch, results["A1"].MatchState);
        Assert.Equal(MatchState.FullMatch, results["B1"].MatchState);
        Assert.Equal(MatchState.PartialMatch, results["C1"].MatchState);
        Assert.Equal(MatchState.FullMatch, results["D1"].MatchState);
        Assert.Equal(MatchState.FullMatch, results["E1"].MatchState);
    }
}
