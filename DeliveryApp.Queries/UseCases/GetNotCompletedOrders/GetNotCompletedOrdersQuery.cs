using MediatR;

namespace DeliveryApp.Queries.UseCases.GetNotCompletedOrders;

public class GetNotCompletedOrdersQuery : IRequest<GetNotCompletedOrdersResult>;
