
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Queries.UseCases.GetAllCouriers;
using DeliveryApp.Queries.UseCases.GetNotCompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;
using Location = OpenApi.Models.Location;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController : DefaultApiController
{
    private readonly IMediator _mediator;

    public DeliveryController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> CreateCourier(NewCourier newCourier)
    {
        // TODO: непонятно
        return NotFound();
    }

    public override async Task<IActionResult> CreateOrder()
    {
        // TODO: добавить GlobalExceptionHandler
        
        var orderId = Guid.CreateVersion7();
        var street = "Unknown";
        var command = CreateOrderCommand.Create(orderId, street, Volume.Create(5));
        await _mediator.Send(command);
        
        return Ok();
    }

    public override async Task<IActionResult> GetOrders()
    {
        var getActiveOrdersQuery = new GetNotCompletedOrdersQuery();
        var response = await _mediator.Send(getActiveOrdersQuery);
        
        if (response == null) 
            return NotFound();
        
        var model = response.Orders.Select(o => new Order
        {
            Id = o.Id,
            Location = new Location
            {
                X = o.LocationDto.X, 
                Y = o.LocationDto.Y
            }
        });
        
        return Ok(model);
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var getAllCouriersQuery = new GetAllCouriersQuery();
        var response = await _mediator.Send(getAllCouriersQuery);
        if (response == null) 
            return NotFound();
        
        var model = response.Couriers.Select(c => new Courier
        {
            Id = c.Id,
            Name = c.Name,
            Location = new Location
            {
                X = c.Location.X, 
                Y = c.Location.Y
            }
        });
        
        return Ok(model);
    }
}
