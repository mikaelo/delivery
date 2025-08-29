using DeliveryApp.Core.Application.UseCases.Exceptions;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public sealed class OrderAlreadyExistsException(Guid orderId) 
    : AppException($"Order {orderId} already exists");
