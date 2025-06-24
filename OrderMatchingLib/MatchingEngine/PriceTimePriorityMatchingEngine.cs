using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PriceTimePriorityMatchingEngine : IMatchingEngine
{
    public IEnumerable<OrderMatchResult> Match(IEnumerable<Order> orders)
    {
        if (orders == null) throw new ArgumentNullException(nameof(orders));

        var results = orders.ToDictionary(o => o.OrderId, o => new OrderMatchResult
        {
            CompanyId = o.CompanyId,
            OrderId = o.OrderId,
            Direction = o.Direction,
            Volume = o.Volume,
            Notional = o.Notional,
            MatchState = MatchState.NoMatch
        });

        var buys = orders.Where(o => o.Direction == OrderDirection.Buy)
                          .OrderByDescending(o => o.Notional)
                          .ThenBy(o => o.OrderDateTime).ToList();

        var sells = orders.Where(o => o.Direction == OrderDirection.Sell)
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

                results[buy.OrderId].MatchState = results[buy.OrderId].Volume == 0 ? MatchState.FullMatch : MatchState.PartialMatch;
                results[sell.OrderId].MatchState = results[sell.OrderId].Volume == 0 ? MatchState.FullMatch : MatchState.PartialMatch;

                if (results[sell.OrderId].Volume == 0)
                    break;
            }
        }

        return results.Values;
    }

    public Task<IEnumerable<OrderMatchResult>> MatchAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default)
    {
        // Execute the CPU-bound matching algorithm on the thread pool to avoid blocking caller threads
        return Task.Run(() => Match(orders), cancellationToken);
    }
}
