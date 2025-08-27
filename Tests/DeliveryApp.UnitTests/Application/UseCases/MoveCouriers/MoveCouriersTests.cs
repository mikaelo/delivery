using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases.MoveCouriers;

public class MoveCouriersShould
{
    private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
    private readonly ICourierRepository _courierRepository = Substitute.For<ICourierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    
    [Fact]
    public async Task CompleteSuccessfullyIfNoAssignedOrders()
    {
        //Arrange
        
        _orderRepositoryMock.GetAllInAssignedStatus()
            .Returns(Task.FromResult<IReadOnlyCollection<Order>>(new List<Order>()));

        //Act

        var command = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_unitOfWork, _orderRepositoryMock, _courierRepository);
        
        await handler.Handle(command, CancellationToken.None);
        
        //Assert

        _courierRepository.Received(0);
        _unitOfWork.Received(0);
    }
    
    [Fact]
    public async Task MoveCourierToOrderLocation()
    {
        //Arrange

        var order = Order.Create(Guid.CreateVersion7(), Location.Create(3,3), Volume.Create(5));
        var courier = Courier.Create("courier", Speed.Create(1), Location.Create(1, 1));
        order.Assign(courier);
        courier.TakeOrder(order);
        
        _orderRepositoryMock.GetAllInAssignedStatus()
            .Returns(Task.FromResult<IReadOnlyCollection<Order>>(new List<Order>()
            {
                order
            }));
        
        _courierRepository.GetAsync(Arg.Is(courier.Id))
            .Returns(Task.FromResult(Maybe<Courier>.From(courier)));
        
        //Act

        var command = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_unitOfWork, _orderRepositoryMock, _courierRepository);
        
        await handler.Handle(command, CancellationToken.None);
        
        //Assert
        
        _orderRepositoryMock.Received(1);
        _courierRepository.Received(1);
        _unitOfWork.Received(1);

        courier.Location.Should().NotBeSameAs(Location.Create(1, 1));
    }
    
    [Fact]
    public async Task CompleteIfReachedDestination()
    {
        //Arrange

        var order = Order.Create(Guid.CreateVersion7(), Location.Create(2,2), Volume.Create(5));
        var courier = Courier.Create("courier", Speed.Create(2), Location.Create(1, 1));
        order.Assign(courier);
        courier.TakeOrder(order);
        
        _orderRepositoryMock.GetAllInAssignedStatus()
            .Returns(Task.FromResult<IReadOnlyCollection<Order>>(new List<Order>()
            {
                order
            }));
        
        _courierRepository.GetAsync(Arg.Is(courier.Id))
            .Returns(Task.FromResult(Maybe<Courier>.From(courier)));
        
        //Act

        var command = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_unitOfWork, _orderRepositoryMock, _courierRepository);
        
        await handler.Handle(command, CancellationToken.None);
        
        //Assert
        
        _orderRepositoryMock.Received(1);
        _courierRepository.Received(1);
        _unitOfWork.Received(1);

        courier.Location.Should().Be(order.Location);
        order.Status.Should().Be(OrderStatus.Completed);
    }
}