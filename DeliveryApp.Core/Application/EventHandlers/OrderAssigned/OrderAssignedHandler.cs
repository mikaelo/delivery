using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.EventHandlers.OrderAssigned;

public class OrderAssignedHandler(IOrderEventsProducer producer)
    : INotificationHandler<OrderAssignedDomainEvent>
{
    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly IOrderEventsProducer _producer = producer;

    public Task Handle(OrderAssignedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        return _producer.Publish(domainEvent, cancellationToken);
    }
}