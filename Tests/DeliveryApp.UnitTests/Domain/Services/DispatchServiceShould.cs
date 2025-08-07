using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceShould
{
    [Fact]
    public void ThrowArgumentNullException_WhenOrderIsNull()
    {
        // Arrange
        var courier1 = Courier.Create("Courier 1", Speed.Create(1), Location.Create(1, 1));
        var courier2 = Courier.Create("Courier 2", Speed.Create(2), Location.Create(2, 2));
        var courier3 = Courier.Create("Courier 3", Speed.Create(3), Location.Create(3, 3));

        Courier[] couriers = [courier1, courier2, courier3];

        var dispatchService = new DispatchService();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => dispatchService.Dispatch(null, couriers));
    }
    
    [Fact]
    public void ThrowArgumentNullException_WhenCouriersListIsNull()
    {
        // Arrange
        var order = Order.Create(Guid.CreateVersion7(), Location.Create(1, 1), Volume.Create(5));

        var dispatchService = new DispatchService();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => dispatchService.Dispatch(order, null));
    }
    
    [Fact]
    public void ReturnNoneCourierFound_WhenCouriersListIsEmpty()
    {
        // Arrange
        var order = Order.Create(Guid.CreateVersion7(), Location.Create(1, 1), Volume.Create(5));
        
        Courier[] couriers = [];
        
        var dispatchService = new DispatchService();
        
        // Act 
        var fastestCourier = dispatchService.Dispatch(order, couriers);
        
        // Assert
        Assert.False(fastestCourier.HasValue);
    }
    
    [Fact]
    public void ReturnNoneCourierFound_WhenNoneAvailableCouriersFound()
    {
        // Arrange
        var order = Order.Create(Guid.CreateVersion7(), Location.Create(1, 1), Volume.Create(30));

        var courier1 = Courier.Create("Courier 1", Speed.Create(1), Location.Create(1, 1));
        var courier2 = Courier.Create("Courier 2", Speed.Create(2), Location.Create(2, 2));
        var courier3 = Courier.Create("Courier 3", Speed.Create(3), Location.Create(3, 3));
        
        Courier[] couriers = [courier1, courier2, courier3];

        var dispatchService = new DispatchService();
        
        // Act & Assert
        var fastestCourier = dispatchService.Dispatch(order, couriers);
        
        // Assert
        Assert.False(fastestCourier.HasValue);
    }
    
    [Fact]
    public void ReturnFastestCourier()
    {
        // Arrange
        var order = Order.Create(Guid.CreateVersion7(), Location.Create(5, 5), Volume.Create(5));

        var courier1 = Courier.Create("Courier 1", Speed.Create(1), Location.Create(1, 1));
        var courier2 = Courier.Create("Courier 2", Speed.Create(2), Location.Create(1, 1));
        var courier3 = Courier.Create("Courier 3", Speed.Create(3), Location.Create(1, 1));
        
        Courier[] couriers = [courier1, courier2, courier3];

        var dispatchService = new DispatchService();
        
        // Act & Assert
        var result = dispatchService.Dispatch(order, couriers);
        
        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(courier3.Id, result.Value.Id);
    }
    
    [Fact]
    public void ReturnNearestCourier()
    {
        // Arrange
        var order = Order.Create(Guid.CreateVersion7(), Location.Create(5, 5), Volume.Create(5));

        var courier1 = Courier.Create("Courier 1", Speed.Create(1), Location.Create(4, 4));
        var courier2 = Courier.Create("Courier 2", Speed.Create(2), Location.Create(1, 1));
        var courier3 = Courier.Create("Courier 3", Speed.Create(3), Location.Create(1, 1));
        
        Courier[] couriers = [courier1, courier2, courier3];

        var dispatchService = new DispatchService();
        
        // Act & Assert
        var result = dispatchService.Dispatch(order, couriers);
        
        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(courier1.Id, result.Value.Id);
    }
}