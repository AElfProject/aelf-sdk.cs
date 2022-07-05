using AElf.Client.CrossChain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace CrossChainManager;

public class CrossChainManagerService : ITransientDependency
{
    private readonly ICrossChainService _crossChainService;
    private IServiceScopeFactory ServiceScopeFactory { get; }

    public ILogger<CrossChainManagerService> Logger { get; set; }

    public CrossChainManagerService(ICrossChainService crossChainService, IServiceScopeFactory serviceScopeFactory)
    {
        _crossChainService = crossChainService;
        ServiceScopeFactory = serviceScopeFactory;
    }

    public async Task<long> GetSyncedHeightByChainId(int chainId)
    {
        var height = await _crossChainService.GetSyncedHeightByChainId(chainId);
        Logger.LogInformation("Synced height of chain id {ChainId}: {Height}", chainId, height);
        return height;
    }
}