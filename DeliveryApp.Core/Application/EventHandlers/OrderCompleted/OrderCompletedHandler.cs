using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.EventHandlers.OrderCompleted;

public class OrderCompletedHandler : INotificationHandler<OrderCompletedDomainEvent>
{
    private readonly IOrderEventsProducer _producer;

    // ReSharper disable once ConvertToPrimaryConstructor
    public OrderCompletedHandler(IOrderEventsProducer producer)
    {
        _producer = producer;
    }
    
    public Task Handle(OrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        return _producer.Publish(domainEvent, cancellationToken);
    }
}