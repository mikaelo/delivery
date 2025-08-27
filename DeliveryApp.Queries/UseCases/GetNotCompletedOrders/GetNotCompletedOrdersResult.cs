namespace DeliveryApp.Queries.UseCases.GetNotCompletedOrders;

public class GetNotCompletedOrdersResult
{
    public List<OrderDto> Orders { get; } = new();
    
    public GetNotCompletedOrdersResult(List<OrderDto> orders)
    {
        Orders.AddRange(orders);
    }
    
    private GetNotCompletedOrdersResult()
    {
    }
    
    public static GetNotCompletedOrdersResult None => new GetNotCompletedOrdersResult();
}

public class OrderDto
{
    public Guid Id { get; set; }
    public LocationDto LocationDto { get; set; }
}
