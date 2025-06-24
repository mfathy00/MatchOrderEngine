# Order Matching Engine Console Sample

Simple console application demonstrating how to
load an order book from a CSV file and execute the matching algorithms defined
in the library.

## Building

```
dotnet build
```

## Running the console

```
dotnet run --project ConsoleApp -- PriceTime SampleData/large_orders.csv
```

Replace `PriceTime` with `ProRata` to use a different strategy.

The `SampleData/large_orders.csv` file contains a generated order book with 2000
entries (1000 buy orders followed by 1000 sell orders) which can be used for
stress testing the engine.