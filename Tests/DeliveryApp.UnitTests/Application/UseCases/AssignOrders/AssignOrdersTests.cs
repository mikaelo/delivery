using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases.AssignOrders;

public class AssignOrdersShould
{
    private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
    private readonly ICourierRepository _courierRepository = Substitute.For<ICourierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    
    
    [Fact]
    public async Task AssignCourierToOrder()
    {
        //Arrange
        
        var order = Order.Create(Guid.CreateVersion7(), Location.Create(3,3), Volume.Create(5));
        var courier = Courier.Create("courier", Speed.Create(1), Location.Create(1, 1));
        
        _orderRepositoryMock.GetFirstInCreatedStatusAsync()
            .Returns(Maybe<Order>.From(order));

        var freeCouriers = (new List<Courier> { courier }).AsReadOnly();
        
        _courierRepository.FindAllFree()
            .Returns(freeCouriers);

        var dispatchService = new DispatchService();
        
        //Act

        var command = new AssignOrdersCommand();
        var handler = new AssignOrdersHandler(_unitOfWork, _orderRepositoryMock, _courierRepository, dispatchService);
        
        await handler.Handle(command, CancellationToken.None);
        
        //Assert
        
        _orderRepositoryMock.Received(1);
        _courierRepository.Received(1);
        _unitOfWork.Received(1);

        order.CourierId.Should().Be(courier.Id);
    }
}