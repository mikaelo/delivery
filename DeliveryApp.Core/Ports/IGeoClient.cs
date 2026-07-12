using DeliveryApp.Core.Domain.Model.SharedKernel;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    Task<Location> GetLocation(string street, CancellationToken cancellationToken = default);
}