using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases.CreateOrder;

public class CreateOrderHandlerShould
{
    private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGeoClient _geoClientMock = Substitute.For<IGeoClient>();
    
    private Maybe<Order> EmptyOrder() =>  Maybe<Order>.None;
    private Maybe<Order> ExistedOrder() => Order.Create(Guid.NewGuid(), Location.Create(1, 1),Volume.Create(5));
    
    [Fact]
    public async Task ThrowIfOrderAlreadyExists()
    {
        //Arrange
        
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(ExistedOrder()));
        
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));
        
        _geoClientMock.GetLocation(Arg.Any<string>())
            .Returns(Task.FromResult(Location.Create(2, 2)));

        //Act
        
        var command = CreateOrderCommand.Create(Guid.NewGuid(), "100000", Volume.Create(5));
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock, _geoClientMock);
        

        //Assert
        
        await Assert.ThrowsAsync<OrderAlreadyExistsException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateOrderSuccessfully()
    {
        //Arrange
        
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(EmptyOrder()));
        
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));
        
        _geoClientMock.GetLocation(Arg.Any<string>())
            .Returns(Task.FromResult(Location.Create(2, 2)));

        //Act
        
        var command = CreateOrderCommand.Create(Guid.NewGuid(), "100000",Volume.Create(5));
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock, _geoClientMock);
        await handler.Handle(command, CancellationToken.None);

        //Assert
        
        _orderRepositoryMock.Received(1);
        _unitOfWork.Received(1);
    }
}
