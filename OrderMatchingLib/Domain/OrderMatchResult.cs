using System.Collections.Generic;

public class OrderMatchResult
{
    public string CompanyId { get; init; }
    public string OrderId { get; init; }
    public OrderDirection Direction { get; init; }
    public int Volume { get; set; }
    public decimal Notional { get; init; }
    public MatchState MatchState { get; set; }
    public List<(string MatchedOrderId, decimal Notional, int Volume)> Matches { get; } = new();
}

public enum MatchState
{
    NoMatch,
    FullMatch,
    PartialMatch,
    InvalidOrder
}

public interface IMatchingEngine
{
    /// <summary>
    /// Synchronously matches a collection of orders.
    /// </summary>
    IEnumerable<OrderMatchResult> Match(IEnumerable<Order> orders);

    /// <summary>
    /// Asynchronously matches a collection of orders. The default
    /// implementation executes the matching logic on the thread pool
    /// to avoid blocking caller threads.
    /// </summary>
    Task<IEnumerable<OrderMatchResult>> MatchAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default);
}
