using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ProRataMatchingEngineTests
{
    [Fact]
    public async Task MatchesAccordingToProRata()
    {
        var orders = new[]
        {
            new Order("A", "A1", OrderDirection.Buy, 50, 5.00m, DateTime.Parse("09:27:43")),
            new Order("B", "B1", OrderDirection.Buy, 200, 5.00m, DateTime.Parse("10:21:46")),
            new Order("C", "C1", OrderDirection.Sell, 200, 5.00m, DateTime.Parse("10:26:18")),
        };

        var engine = new ProRataMatchingEngine();
        var results = (await engine.MatchAsync(orders).ConfigureAwait(false)).ToDictionary(r => r.OrderId);

        Assert.Equal(MatchState.PartialMatch, results["A1"].MatchState);
        Assert.Equal(MatchState.PartialMatch, results["B1"].MatchState);
        Assert.Equal(MatchState.FullMatch, results["C1"].MatchState);
    }
}
