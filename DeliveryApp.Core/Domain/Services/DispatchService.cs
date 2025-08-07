using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
    public Maybe<Courier> Dispatch(Order order, IEnumerable<Courier> couriers)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(couriers);
        
        var availableCouriers = couriers.Where(c => c.CanTakeOrder(order)).ToList();
        
        if (!availableCouriers.Any())
            return Maybe<Courier>.None;

        var fastestCourier = availableCouriers
            .OrderBy(cs => cs.CalculateTimeToLocation(order.Location))
            .First();
        
        //order.Assign(fastestCourier);
        fastestCourier.TakeOrder(order); // внутри курьера в order прописывается courier ?
        
        return fastestCourier;
    }
}