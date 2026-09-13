using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyCloset.Services;

public sealed class CosmosHealthCheck : IHealthCheck
{
    private readonly CosmosClient _cosmosClient;

    public CosmosHealthCheck(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cosmosClient.ReadAccountAsync();
            return HealthCheckResult.Healthy("Cosmos DB is reachable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Cosmos DB is unavailable.", exception);
        }
    }
}