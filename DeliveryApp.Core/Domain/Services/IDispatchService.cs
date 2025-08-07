using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services;

public interface IDispatchService
{
    Maybe<Courier> Dispatch(Order order, IEnumerable<Courier> couriers);
}