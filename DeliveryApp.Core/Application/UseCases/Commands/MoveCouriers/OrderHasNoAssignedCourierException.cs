using DeliveryApp.Core.Application.UseCases.Exceptions;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public sealed class OrderHasNoAssignedCourierException(Guid orderId)
    : AppException($"Order {orderId} has no assigned couriers");