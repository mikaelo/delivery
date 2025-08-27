namespace DeliveryApp.Queries.UseCases.GetAllCouriers;

public class GetAllCouriersResult
{
    public List<CourierDto> Couriers { get; } = new();
    
    public GetAllCouriersResult(List<CourierDto> couriers)
    {
        Couriers.AddRange(couriers);
    }
    
    private GetAllCouriersResult()
    {
    }
    
    public static GetAllCouriersResult None => new GetAllCouriersResult();
}

public class CourierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LocationDto Location { get; set; }
}
