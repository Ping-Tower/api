using Xunit;

namespace Api.IntegrationTests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ApiIntegrationCollection : ICollectionFixture<PingTowerApiFactory>
{
    public const string Name = "api-integration";
}
