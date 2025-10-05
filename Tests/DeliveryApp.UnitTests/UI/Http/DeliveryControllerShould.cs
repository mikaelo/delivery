﻿using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace DeliveryApp.UnitTests.UI.Http;

public class DeliveryControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task CreateOrderCorrectly()
    {
        // Arrange
        _mediator.Send(Arg.Any<CreateOrderCommand>())
            .Returns(Unit.Value);

        // Act
        var deliveryController = new DeliveryController(_mediator);
        var result = await deliveryController.CreateOrder();

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}

