using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Queries.UseCases.GetAllCouriers;

public class GetAllCouriersHandler : IRequestHandler<GetAllCouriersQuery, GetAllCouriersResult>
{
    private readonly string _connectionString;

    public GetAllCouriersHandler(string connectionString)
    {
        _connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<GetAllCouriersResult> Handle(GetAllCouriersQuery message, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var result = await connection.QueryAsync<dynamic>(
            @"SELECT id, name, location_x, location_y FROM public.couriers");

        var dbCouriers = result.AsList();
        
        if (dbCouriers.Count == 0)
            return GetAllCouriersResult.None;

        var couriers = dbCouriers.Select(Mapper.MapToCourier).ToList();
        
        return new GetAllCouriersResult(couriers);
    }

    private static class Mapper
    {
        public static CourierDto MapToCourier(dynamic result)
        {
            var location = new LocationDto()
            {
                X = result.location_x, 
                Y = result.location_y
            };
        
            var courier = new CourierDto
            {
                Id = result.id, 
                Name = result.name, 
                Location = location
            };
        
            return courier;
        }
    }
}
