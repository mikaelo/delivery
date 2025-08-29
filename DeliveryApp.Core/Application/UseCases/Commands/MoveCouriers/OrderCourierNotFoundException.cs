using DeliveryApp.Core.Application.UseCases.Exceptions;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public sealed class OrderCourierNotFoundException(Guid orderId, Guid courierId)
: AppException($"Courier {courierId} from order {orderId} was not found.");