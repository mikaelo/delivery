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
    private readonly IGeoClient _geoClient;
    
    private static readonly Random Random = new Random();
        
    public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IGeoClient geoClient)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _geoClient = geoClient  ?? throw new ArgumentNullException(nameof(geoClient));
    }

    public async Task<Unit> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var getOrderResult = await _orderRepository.GetAsync(message.OrderId);
        if (getOrderResult.HasValue)
            throw new OrderAlreadyExistsException(message.OrderId);
        
        var location =  await _geoClient.GetLocation(message.Index, cancellationToken);
        
        var order = Order.Create(message.OrderId, location, message.Volume);
        
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }

}