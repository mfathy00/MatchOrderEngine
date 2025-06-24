# Order Matching Engine Console Sample

Simple console application demonstrating how to
load an order book from a CSV file and execute the matching algorithms defined
in the library.

## Running the console

```
dotnet run --project ConsoleApp -- PriceTime SampleData/sample_orders.csv
```

Replace `PriceTime` with `ProRata` to use a different strategy.  The
`SampleData/sample_orders.csv` file contains a handful of orders and is useful
for quickly trying out the matching engine.  A much larger order book for
stress testing is provided in `SampleData/large_orders.csv` which contains 2000
entries (1000 buy orders followed by 1000 sell orders).
