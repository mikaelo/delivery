using DeliveryApp.Core.Domain.Model.SharedKernel;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Unit>
{
    public Guid OrderId { get; }
    
    public string Index { get; }
    
    public Volume Volume { get; }

    
    private CreateOrderCommand(Guid orderId, string index, Volume volume)
    {
        OrderId = orderId;
        Index = index;
        Volume = volume;
    }
    
    public static CreateOrderCommand Create(Guid orderId, string index, Volume volume)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(orderId, Guid.Empty);
        ArgumentException.ThrowIfNullOrEmpty(index);
        
        return new CreateOrderCommand(orderId, index, volume);
    }

}