using DeliveryApp.Core.Application.UseCases.Exceptions;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;

public sealed class FreeCourierNotFoundException() 
    : AppException("No free courier found");