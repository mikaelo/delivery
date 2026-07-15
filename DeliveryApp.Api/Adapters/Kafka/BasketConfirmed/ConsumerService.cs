using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using Queues.Basket.Events;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

public class ConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, byte[]> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _topic;

    public ConsumerService(IServiceScopeFactory scopeFactory, IOptions<Settings> settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Value.MessageBrokerHost))
            throw new ArgumentException(nameof(settings.Value.MessageBrokerHost));
        
        if (string.IsNullOrWhiteSpace(settings.Value.BasketConfirmedTopic))
            throw new ArgumentException(nameof(settings.Value.BasketConfirmedTopic));

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = settings.Value.MessageBrokerHost,
            GroupId = "BasketConsumerGroup",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _consumer = new ConsumerBuilder<Ignore, byte[]>(consumerConfig).Build();
        _topic = settings.Value.BasketConfirmedTopic;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Позволяем host продолжить запуск Kestrel.
        await Task.Yield();
        
        _consumer.Subscribe(_topic);
        
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult.IsPartitionEOF)
                    continue;

                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                BasketConfirmedIntegrationEvent evt;

                try
                {
                    evt = BasketConfirmedIntegrationEvent.Parser.ParseFrom(consumeResult.Message.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    continue;
                }

                try
                {
                    var command = CreateOrderCommand.Create(
                        Guid.Parse(evt.BasketId), 
                        evt.Address.Street, 
                        Volume.Create(evt.Volume));
                
                    await mediator.Send(command, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue; // А не зависнет ли обработка на одном и том же сообщении ?
                }
                
                _consumer.StoreOffset(consumeResult);
            }
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}