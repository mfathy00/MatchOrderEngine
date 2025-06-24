using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class LoggingMatchingEngineDecorator : IMatchingEngine
{
    private readonly IMatchingEngine _inner;
    private readonly ILogger _logger;

    public LoggingMatchingEngineDecorator(IMatchingEngine inner, ILogger logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<OrderMatchResult> Match(IEnumerable<Order> orders)
    {
        return _logger.LogExecution("Match", () => _inner.Match(orders));
    }

    public Task<IEnumerable<OrderMatchResult>> MatchAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default)
    {
        return _logger.LogExecutionAsync("MatchAsync", () => _inner.MatchAsync(orders, cancellationToken));
    }
}
