using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PriceTimePriorityMatchingEngine : IMatchingEngine
{
    private readonly ILogger? _logger;

    public PriceTimePriorityMatchingEngine(ILogger? logger = null)
    {
        _logger = logger;
    }
    public IEnumerable<OrderMatchResult> Match(IEnumerable<Order> orders)
    {
        if (orders == null) throw new ArgumentNullException(nameof(orders));

        try
        {
            var orderList = orders.ToList();
            _logger?.LogInfo($"Starting PriceTime match with {orderList.Count} orders");

        var results = orderList.ToDictionary(o => o.OrderId, o => new OrderMatchResult
        {
            CompanyId = o.CompanyId,
            OrderId = o.OrderId,
            Direction = o.Direction,
            Volume = o.Volume,
            Notional = o.Notional,
            MatchState = MatchState.NoMatch
        });

        var buys = orderList.Where(o => o.Direction == OrderDirection.Buy)
                          .OrderByDescending(o => o.Notional)
                          .ThenBy(o => o.OrderDateTime).ToList();

        var sells = orderList.Where(o => o.Direction == OrderDirection.Sell)
                           .OrderBy(o => o.OrderDateTime).ToList();

        foreach (var sell in sells)
        {
            foreach (var buy in buys.ToList())
            {
                if (buy.Notional < sell.Notional || results[buy.OrderId].Volume == 0)
                    continue;

                var matchVolume = Math.Min(results[buy.OrderId].Volume, results[sell.OrderId].Volume);
                results[buy.OrderId].Volume -= matchVolume;
                results[sell.OrderId].Volume -= matchVolume;
                results[buy.OrderId].Matches.Add((sell.OrderId, buy.Notional, matchVolume));
                results[sell.OrderId].Matches.Add((buy.OrderId, buy.Notional, matchVolume));

                _logger?.LogInfo($"Matched {buy.OrderId} -> {sell.OrderId} vol {matchVolume} @ {buy.Notional}");

                results[buy.OrderId].MatchState = results[buy.OrderId].Volume == 0 ? MatchState.FullMatch : MatchState.PartialMatch;
                results[sell.OrderId].MatchState = results[sell.OrderId].Volume == 0 ? MatchState.FullMatch : MatchState.PartialMatch;

                if (results[sell.OrderId].Volume == 0)
                    break;
            }
        }

            _logger?.LogInfo("PriceTime match completed");
            return results.Values;
        }
        catch (Exception ex)
        {
            _logger?.LogError("PriceTime match failed", ex);
            throw;
        }
    }

    public async Task<IEnumerable<OrderMatchResult>> MatchAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default)
    {
        try
        {
            // Execute the CPU-bound matching algorithm on the thread pool to avoid blocking caller threads
            return await Task.Run(() => Match(orders), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError("PriceTime match async failed", ex);
            throw;
        }
    }
}
