using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports;

public interface IOrderEventsProducer
{
    Task Publish(OrderAssignedDomainEvent domainEvent, CancellationToken cancellationToken);
    Task Publish(OrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken);
}