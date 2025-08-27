using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    private static readonly Random Random = new Random();
        
    public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<Unit> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var getOrderResult = await _orderRepository.GetAsync(message.OrderId);
        if (getOrderResult.HasValue)
            throw new ArgumentNullException($"Order {message.OrderId} already exists");
        
        var location = Location.CreateRandom(Random);
        
        var order = Order.Create(message.OrderId, location, message.Volume);
        
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }

}