using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class OrderBookLoaderTests
{
    [Fact]
    public void LoadFromCsv_ThrowsException_WhenLinesIsNull()
    {
        Assert.Throws<ArgumentException>(() => OrderBookLoader.LoadFromCsv(null!));
    }

    [Fact]
    public void LoadFromCsv_ThrowsException_WhenLinesIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => OrderBookLoader.LoadFromCsv(Array.Empty<string>()));
    }

    [Fact]
    public void LoadFromCsv_ThrowsException_WhenOnlyHeaderPresent()
    {
        var lines = new[] { "CompanyId,OrderId,Direction,Volume,Notional,OrderDateTime" };
        Assert.Throws<ArgumentException>(() => OrderBookLoader.LoadFromCsv(lines));
    }

    [Fact]
    public void LoadFromCsv_ThrowsException_WhenIncorrectColumnCount()
    {
        var lines = new[]
        {
            "CompanyId,OrderId,Direction,Volume,Notional,OrderDateTime",
            "A,A1,Buy,100" // Missing columns
        };
        
        var exception = Assert.Throws<FormatException>(() => OrderBookLoader.LoadFromCsv(lines).ToList());
        Assert.Contains("Error parsing line 2", exception.Message);
    }

    [Fact]
    public void LoadFromCsv_ThrowsException_WhenInvalidDirection()
    {
        var lines = new[]
        {
            "CompanyId,OrderId,Direction,Volume,Notional,OrderDateTime",
            "A,A1,InvalidDirection,100,10.5,2020-01-01T09:27:43Z"
        };
        
        var exception = Assert.Throws<FormatException>(() => OrderBookLoader.LoadFromCsv(lines).ToList());
        Assert.Contains("Error parsing line 2", exception.Message);
    }

    [Fact]
    public void LoadFromCsv_ThrowsException_WhenInvalidVolume()
    {
        var lines = new[]
        {
            "CompanyId,OrderId,Direction,Volume,Notional,OrderDateTime",
            "A,A1,Buy,InvalidVolume,10.5,2020-01-01T09:27:43Z"
        };
        
        var exception = Assert.Throws<FormatException>(() => OrderBookLoader.LoadFromCsv(lines).ToList());
        Assert.Contains("Error parsing line 2", exception.Message);
    }

    [Fact]
    public void LoadFromCsv_HandlesWhitespace()
    {
        var lines = new[]
        {
            "CompanyId,OrderId,Direction,Volume,Notional,OrderDateTime",
            " A , A1 , Buy , 100 , 10.5 , 2020-01-01T09:27:43Z "
        };
        
        var orders = OrderBookLoader.LoadFromCsv(lines).ToList();
        
        Assert.Single(orders);
        Assert.Equal("A", orders[0].CompanyId);
        Assert.Equal("A1", orders[0].OrderId);
    }

    [Fact]
    public async Task LoadFromCsvAsync_ThrowsException_WhenFileNotExists()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            OrderBookLoader.LoadFromCsvAsync("nonexistent.csv"));
    }

    [Fact]
    public async Task LoadFromCsvAsync_ThrowsException_WhenPathIsNull()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => 
            OrderBookLoader.LoadFromCsvAsync(null!));
    }

    [Fact]
    public async Task LoadFromCsvAsync_ThrowsException_WhenPathIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => 
            OrderBookLoader.LoadFromCsvAsync(""));
    }
} 