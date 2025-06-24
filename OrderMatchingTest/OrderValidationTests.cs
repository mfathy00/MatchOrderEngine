using System;
using Xunit;

public class OrderValidationTests
{
    [Fact]
    public void Order_ThrowsException_WhenCompanyIdIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Order(
            null!,
            "O1",
            OrderDirection.Buy,
            100,
            10.5m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_ThrowsException_WhenCompanyIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Order(
            "",
            "O1",
            OrderDirection.Buy,
            100,
            10.5m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_ThrowsException_WhenOrderIdIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Order(
            "C1",
            null!,
            OrderDirection.Buy,
            100,
            10.5m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_ThrowsException_WhenVolumeIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(
            "C1",
            "O1",
            OrderDirection.Buy,
            0,
            10.5m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_ThrowsException_WhenVolumeIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(
            "C1",
            "O1",
            OrderDirection.Buy,
            -100,
            10.5m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_ThrowsException_WhenNotionalIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(
            "C1",
            "O1",
            OrderDirection.Buy,
            100,
            0m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_ThrowsException_WhenNotionalIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(
            "C1",
            "O1",
            OrderDirection.Buy,
            100,
            -10.5m,
            DateTime.Now
        ));
    }

    [Fact]
    public void Order_CreatedSuccessfully_WhenAllParametersValid()
    {
        var order = new Order(
            "C1",
            "O1",
            OrderDirection.Buy,
            100,
            10.5m,
            DateTime.Now
        );

        Assert.Equal("C1", order.CompanyId);
        Assert.Equal("O1", order.OrderId);
        Assert.Equal(OrderDirection.Buy, order.Direction);
        Assert.Equal(100, order.Volume);
        Assert.Equal(10.5m, order.Notional);
    }
} 