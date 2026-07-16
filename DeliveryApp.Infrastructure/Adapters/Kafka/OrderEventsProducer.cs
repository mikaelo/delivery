using System.Text;
using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Queues.Order.Events;

namespace DeliveryApp.Infrastructure.Adapters.Kafka;

public sealed class OrderEventsProducer : IOrderEventsProducer
{
    private readonly ProducerConfig _config;
    private readonly string _topicName;

    public OrderEventsProducer(IOptions<Settings> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.MessageBrokerHost))
            throw new ArgumentException(nameof(options.Value.MessageBrokerHost));
        
        if (string.IsNullOrWhiteSpace(options.Value.OrderStatusChangedTopic))
            throw new ArgumentException(nameof(options.Value.OrderStatusChangedTopic));

        _config = new ProducerConfig
        {
            BootstrapServers = options.Value.MessageBrokerHost
        };
        _topicName = options.Value.OrderStatusChangedTopic;
    }

    public async Task Publish(OrderAssignedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderAssignedIntegrationEvent
        {
            OrderId = domainEvent.OrderId.ToString()
        };
        
        await Produce(
            key: domainEvent.EventId.ToString(),
            integrationEvent,
            domainEvent,
            cancellationToken);
    }

    public async Task Publish(OrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderCompletedIntegrationEvent
        {
            OrderId = domainEvent.OrderId.ToString()
        };
        
        await Produce(
            key: domainEvent.EventId.ToString(),
            integrationEvent,
            domainEvent,
            cancellationToken);
    }

    private async Task Produce<TIntegrationEvent, TDomainEvent>(
        string key,
        TIntegrationEvent integrationEvent,
        TDomainEvent domainEvent,
        CancellationToken cancellationToken)
        where TIntegrationEvent : IMessage
    {
        var message = new Message<string, byte[]>
        {
            Key = key,
            Value = integrationEvent.ToByteArray(),
            Headers = new Headers
            {
                { "event-id", Encoding.UTF8.GetBytes(key) },
                { "event-type", Encoding.UTF8.GetBytes(domainEvent!.GetType().Name) },
                { "occurred-at", Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) },
                { "content-type", "application/x-protobuf"u8.ToArray() },
                { "debug-json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent)) }
            }
        };

        using var producer = new ProducerBuilder<string, byte[]>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);
    }
}