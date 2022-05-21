using Volo.Abp.DependencyInjection;

namespace AElf.Client;

public interface IAElfClientProvider
{
    AElfClient GetClient(string? alias = null, string? environment = null, int? chainId = null, string? chainType = null);

    void SetClient(AElfClient client, string? environment = null, int? chainId = null, string? chainType = null,
        string? alias = null);
}

public class AElfClientProvider : Dictionary<AElfClientInfo, AElfClient>, IAElfClientProvider, ISingletonDependency
{
    public AElfClientProvider()
    {
        var clientBuilder = new AElfClientBuilder();
        SetClient(clientBuilder.UsePublicEndpoint(EndpointType.MainNetMainChain).Build(), "MainNet",
            AElfClientConstants.MainChainId, "MainChain", EndpointType.MainNetMainChain.ToString());
        SetClient(clientBuilder.UsePublicEndpoint(EndpointType.MainNetSidechain).Build(), "MainNet",
            AElfClientConstants.SidechainId, "Sidechain", EndpointType.MainNetSidechain.ToString());
        SetClient(clientBuilder.UsePublicEndpoint(EndpointType.TestNetMainChain).Build(), "TestNet",
            AElfClientConstants.MainChainId, "MainChain", EndpointType.TestNetMainChain.ToString());
        SetClient(clientBuilder.UsePublicEndpoint(EndpointType.TestNetSidechain).Build(), "MainNet",
            AElfClientConstants.SidechainId, "Sidechain", EndpointType.TestNetSidechain.ToString());
        SetClient(clientBuilder.UsePublicEndpoint(EndpointType.Local).Build(), "Local", AElfClientConstants.MainChainId,
            "MainChain", EndpointType.Local.ToString());
    }

    public AElfClient GetClient(string? alias = null, string? environment = null, int? chainId = null,
        string? chainType = null)
    {
        var keys = Keys
            .WhereIf(!alias.IsNullOrWhiteSpace(), c => c.Alias == alias)
            .WhereIf(!environment.IsNullOrWhiteSpace(), c => c.Environment == environment)
            .WhereIf(chainId.HasValue, c => c.ChainId == chainId)
            .WhereIf(!chainType.IsNullOrWhiteSpace(), c => c.ChainType == chainType)
            .ToList();
        if (keys.Count != 1)
        {
            throw new AElfClientException($"Failed to get client of {alias} - {environment} - {chainId} - {chainType}");
        }

        return this[keys.Single()];
    }

    public void SetClient(AElfClient client, string? environment = null, int? chainId = null, string? chainType = null,
        string? alias = null)
    {
        TryAdd(new AElfClientInfo
        {
            Environment = environment,
            ChainId = chainId,
            ChainType = chainType,
            Alias = alias
        }, client);
    }
}

public class AElfClientInfo
{
    public string? Environment { get; set; }
    public int? ChainId { get; set; }
    public string? ChainType { get; set; }
    public string? Alias { get; set; }
}