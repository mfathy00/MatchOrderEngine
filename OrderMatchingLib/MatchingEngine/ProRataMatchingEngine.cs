using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ProRataMatchingEngine : IMatchingEngine
{
    private readonly ILogger? _logger;

    public ProRataMatchingEngine(ILogger? logger = null)
    {
        _logger = logger;
    }
    public IEnumerable<OrderMatchResult> Match(IEnumerable<Order> orders)
    {
        if (orders == null) throw new ArgumentNullException(nameof(orders));

        try
        {
            var orderList = orders.ToList();
            _logger?.LogInfo($"Starting ProRata match with {orderList.Count} orders");

        var results = orderList.ToDictionary(o => o.OrderId, o => new OrderMatchResult
        {
            CompanyId = o.CompanyId,
            OrderId = o.OrderId,
            Direction = o.Direction,
            Volume = o.Volume,
            Notional = o.Notional,
            MatchState = MatchState.NoMatch
        });

        var buyGroups = orderList.Where(o => o.Direction == OrderDirection.Buy)
                               .GroupBy(o => o.Notional);

        var sells = orderList.Where(o => o.Direction == OrderDirection.Sell)
                           .OrderBy(o => o.OrderDateTime).ToList();

        foreach (var group in buyGroups)
        {
            var price = group.Key;
            var buyers = group.OrderBy(o => o.OrderDateTime).ToList();
            var totalVolume = buyers.Sum(o => o.Volume);

            foreach (var sell in sells.Where(s => s.Notional == price).ToList())
            {
                if (totalVolume == 0 || results[sell.OrderId].Volume == 0)
                    continue;

                foreach (var buyer in buyers)
                {
                    var proportion = (decimal)buyer.Volume / totalVolume;
                    var matchVolume = Math.Min((int)(proportion * results[sell.OrderId].Volume), results[buyer.OrderId].Volume);

                    if (matchVolume == 0)
                        continue;

                    results[buyer.OrderId].Volume -= matchVolume;
                    results[sell.OrderId].Volume -= matchVolume;
                    results[buyer.OrderId].Matches.Add((sell.OrderId, buyer.Notional, matchVolume));
                    results[sell.OrderId].Matches.Add((buyer.OrderId, buyer.Notional, matchVolume));

                    _logger?.LogInfo($"Matched {buyer.OrderId} -> {sell.OrderId} vol {matchVolume} @ {buyer.Notional}");

                    results[buyer.OrderId].MatchState = results[buyer.OrderId].Volume == 0 ? MatchState.FullMatch : MatchState.PartialMatch;
                    results[sell.OrderId].MatchState = results[sell.OrderId].Volume == 0 ? MatchState.FullMatch : MatchState.PartialMatch;

                    if (results[sell.OrderId].Volume == 0)
                        break;
                }
            }
        }

            _logger?.LogInfo("ProRata match completed");
            return results.Values;
        }
        catch (Exception ex)
        {
            _logger?.LogError("ProRata match failed", ex);
            throw;
        }
    }

    public async Task<IEnumerable<OrderMatchResult>> MatchAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default)
    {
        try
        {
            // Execute CPU-bound work on the thread pool
            return await Task.Run(() => Match(orders), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError("ProRata match async failed", ex);
            throw;
        }
    }
}
