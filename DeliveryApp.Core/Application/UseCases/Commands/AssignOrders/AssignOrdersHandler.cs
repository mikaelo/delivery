using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;

public class AssignOrdersHandler : IRequestHandler<AssignOrdersCommand, Unit>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IDispatchService _dispatchService;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public AssignOrdersHandler(
        IUnitOfWork unitOfWork, 
        IOrderRepository orderRepository,
        ICourierRepository courierRepository,
        IDispatchService dispatchService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
    }

    public async Task<Unit> Handle(AssignOrdersCommand message, CancellationToken cancellationToken)
    {
        var createdOrder = await _orderRepository.GetFirstInCreatedStatusAsync();
        if (createdOrder.HasNoValue)
            return Unit.Value;
        
        var order = createdOrder.Value;

        var availableCouriers = await _courierRepository.FindAllFree();
        if (availableCouriers.Count == 0) 
            throw new FreeCourierNotFoundException();
        
        var dispatchResult = _dispatchService.Dispatch(order, availableCouriers);
        var courier = dispatchResult.Value;
        
        _courierRepository.Update(courier);
        _orderRepository.Update(order);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
