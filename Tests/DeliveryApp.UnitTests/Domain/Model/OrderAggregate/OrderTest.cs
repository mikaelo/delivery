using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate;

public class OrderShould
{
    private readonly Location _validLocation = Location.Create(1, 1);
    private readonly Volume _validVolume = Volume.Create(5);
    private readonly Guid _validOrderId = Guid.NewGuid();
    private readonly Courier _validCourier = Courier.Create("courier_1", Speed.Create(1), Location.Create(1, 1));

    #region Create Order Tests

    [Fact]
    public void BeCreatedWithValidParameters()
    {
        // Act
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);

        // Assert
        Assert.Equal(_validOrderId, order.Id);
        Assert.Equal(_validLocation, order.Location);
        Assert.Equal(_validVolume, order.Volume);
        Assert.Equal(OrderStatus.Created, order.Status);
        Assert.Null(order.CourierId);
    }

    [Fact]
    public void ThrowExceptionWhenCreatedWithEmptyId()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Order.Create(Guid.Empty, _validLocation, _validVolume));

        Assert.Contains("Order ID cannot be empty", exception.Message);
    }

    [Fact]
    public void ThrowExceptionWhenCreatedWithNullLocation()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            Order.Create(_validOrderId, null, _validVolume));

        Assert.Equal("location", exception.ParamName);
    }
    
    [Fact]
    public void ThrowExceptionWhenCreatedWithNullVolume()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            Order.Create(_validOrderId, _validLocation, null));

        Assert.Equal("volume", exception.ParamName);
    }
    
    #endregion

    #region Assign To Courier Tests

    [Fact]
    public void BeAssignedToCourierWhenInCreatedStatus()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);

        // Act
        order.Assign(_validCourier);

        // Assert
        Assert.Equal(_validCourier.Id, order.CourierId);
        Assert.Equal(OrderStatus.Assigned, order.Status);
    }

    [Fact]
    public void ThrowExceptionWhenAssignedWithEmptyCourierId()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => order.Assign(null));
    }

    [Fact]
    public void ThrowExceptionWhenAssignedToAlreadyAssignedOrder()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);
        order.Assign(_validCourier);

        // Act & Assert
        var anotherCourierId = Courier.Create("courier_2", Speed.Create(2), Location.Create(2, 2));
        var exception = Assert.Throws<InvalidOperationException>(() => order.Assign(anotherCourierId));
    }

    [Fact]
    public void ThrowExceptionWhenAssignedToCompletedOrder()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);
        order.Assign(_validCourier);
        order.Complete();

        // Act & Assert
        var anotherCourierId = Courier.Create("courier_2", Speed.Create(2), Location.Create(2, 2));
        var exception = Assert.Throws<InvalidOperationException>(() => order.Assign(anotherCourierId));
    }

    #endregion

    #region Complete Order Tests

    [Fact]
    public void BeCompletedWhenInAssignedStatus()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);
        order.Assign(_validCourier);

        // Act
        order.Complete();

        // Assert
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(_validCourier.Id, order.CourierId); // CourierId должен остаться
    }

    [Fact]
    public void ThrowExceptionWhenCompletedInCreatedStatus()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => order.Complete());
    }

    [Fact]
    public void ThrowExceptionWhenCompletedTwice()
    {
        // Arrange
        var order = Order.Create(_validOrderId, _validLocation, _validVolume);
        order.Assign(_validCourier);
        order.Complete();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => order.Complete());
    }

    #endregion
}
    