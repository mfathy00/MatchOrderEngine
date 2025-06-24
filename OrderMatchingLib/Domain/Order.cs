public record Order(
    string CompanyId,
    string OrderId,
    OrderDirection Direction,
    int Volume,
    decimal Notional,
    DateTime OrderDateTime
);
