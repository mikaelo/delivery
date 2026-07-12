using Clients.Geo;
using DeliveryApp.Core.Ports;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Location = DeliveryApp.Core.Domain.Model.SharedKernel.Location;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

public class GeoClient : IGeoClient
{
    private readonly string _endpoint;
    private readonly SocketsHttpHandler _socketsHttpHandler;
    private readonly MethodConfig _methodConfig;

    
    public GeoClient(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint)) 
            throw new ArgumentNullException(nameof(endpoint));
        
        _endpoint = endpoint;

        _socketsHttpHandler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };
        
        _methodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }
    
    public async Task<Location> GetLocation(string index, CancellationToken cancellationToken = default)
    {
        using var channel = GrpcChannel.ForAddress(_endpoint, new GrpcChannelOptions
        {
            HttpHandler = _socketsHttpHandler,
            ServiceConfig = new ServiceConfig { MethodConfigs = { _methodConfig } }
        });
        
        var client = new Geo.GeoClient(channel);

        
        var request = new GetGeolocationRequest
        {
            Street = index
        };
        var response = await client.GetGeolocationAsync(request, null, DateTime.UtcNow.AddSeconds(2), cancellationToken);
        
        return Location.Create(response.Location.X, response.Location.Y);
    }
}