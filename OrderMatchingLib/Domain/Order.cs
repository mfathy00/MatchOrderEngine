public class Order
{
    public string CompanyId { get; }
    public string OrderId { get; }
    public OrderDirection Direction { get; }
    public int Volume { get; }
    public decimal Notional { get; }
    public DateTime OrderDateTime { get; }

    public Order(
        string companyId,
        string orderId,
        OrderDirection direction,
        int volume,
        decimal notional,
        DateTime orderDateTime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(companyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(volume);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(notional);

        CompanyId = companyId;
        OrderId = orderId;
        Direction = direction;
        Volume = volume;
        Notional = notional;
        OrderDateTime = orderDateTime;
    }

    public override bool Equals(object? obj) => 
        obj is Order other && 
        CompanyId == other.CompanyId && 
        OrderId == other.OrderId &&
        Direction == other.Direction &&
        Volume == other.Volume &&
        Notional == other.Notional &&
        OrderDateTime == other.OrderDateTime;

    public override int GetHashCode() => 
        HashCode.Combine(CompanyId, OrderId, Direction, Volume, Notional, OrderDateTime);
}
