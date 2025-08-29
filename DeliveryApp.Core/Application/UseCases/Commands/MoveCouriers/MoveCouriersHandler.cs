using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand, Unit>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public MoveCouriersHandler(
        IUnitOfWork unitOfWork, 
        IOrderRepository orderRepository, 
        ICourierRepository courierRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
    }

    public async Task<Unit> Handle(MoveCouriersCommand message, CancellationToken cancellationToken)
    {
        var assignedOrders = await _orderRepository.GetAllInAssignedStatus();
        if (assignedOrders.Count == 0) 
            return Unit.Value;
        
        foreach (var order in assignedOrders)
        {
            // TODO: такой ситуации не должно быть
            if (order.CourierId == null)
                throw new OrderHasNoAssignedCourierException(order.Id);

            var orderCourier = await _courierRepository.GetAsync(order.CourierId.Value);
            if (orderCourier.HasNoValue) // TODO: такой ситуации не должно быть
                throw new OrderCourierNotFoundException(order.Id, order.CourierId.Value);

            var courier = orderCourier.Value;
            courier.Move(order.Location);
            
            if (order.Location == courier.Location)
            {
                order.Complete();
                courier.CompleteOrder(order);
            }

            _courierRepository.Update(courier);
            _orderRepository.Update(order);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
