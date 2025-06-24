**1. Time spent on the engineering task**  
I spent approximately 6 hours designing, implementing, and testing the Order Matching Engine library. I also incorporated the Factory and Strategy design patterns, ensuring strict adherence to SOLID principles, Clean Architecture, and ultra-low-latency requirements.

**2. Additional features if more time were available**  
- **Stress-test benchmark**: Use BenchmarkDotNet to simulate and measure performance with over 1 million records, ensuring the library scales under heavy load.  
- **Dockerization & CI/CD**: Add a Dockerfile and GitHub Actions/Azure DevOps pipeline for automated build, test, and deployment.  
- **Enhanced observability**: Integrate Application Insights like New Relic for real-time monitoring and spike detection.

**3. Most useful features in .NET 8 & Modern C# used in this project**

#### 1. **Nullable Reference Types** 
```csharp
// Project Configuration
<Nullable>enable</Nullable>

// Usage Example from MatchingEngineFactory.cs
public static IMatchingEngine Create(string strategy, ILogger? logger)
{
    // Explicit nullable annotation prevents null reference exceptions
    return logger != null
        ? new LoggingMatchingEngineDecorator(engine, logger)
        : engine;
}
```
**Why I use it:** Eliminates 90% of null reference exceptions at compile time, making the code ultra-reliable for production financial systems where crashes are unacceptable.

#### 2. **Switch Expressions** 
```csharp
// From MatchingEngineFactory.cs
IMatchingEngine engine = strategy switch
{
    "PriceTime" => new PriceTimePriorityMatchingEngine(logger),
    "ProRata" => new ProRataMatchingEngine(logger),
    _ => throw new ArgumentException("Unknown strategy")
};
```
**Why I use it:** more concise than traditional switch statements, improves readability, and reduces bugs in strategy pattern implementation.

#### 3. **Required Properties** 
```csharp
// From OrderMatchResult.cs
public class OrderMatchResult
{
    public required string CompanyId { get; init; }
    public required string OrderId { get; init; }
    // ... other properties
}
```
**Why I use it:** Enforces critical business data integrity at compile time—ensures no order can exist without essential identifiers.

#### 4. **Modern Input Validation** 
```csharp
// From Order.cs constructor
ArgumentException.ThrowIfNullOrWhiteSpace(companyId);
ArgumentException.ThrowIfNullOrWhiteSpace(orderId);
ArgumentOutOfRangeException.ThrowIfNegativeOrZero(volume);
ArgumentOutOfRangeException.ThrowIfNegativeOrZero(notional);
```
**Why I use it:** One-liner validation methods reduce boilerplate code while providing consistent, detailed error messages.


**4. Tracking down a performance issue in production**  
I created detailed logs to track processing duration from start to finish in the current application. This is one method of monitoring performance; another is using observability tools—such as tracing, metrics, and logs—to pinpoint where latency occurs.
