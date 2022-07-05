using AElf.Client.Core;
using AElf.Client.Core.Options;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.CrossChain;

public class CrossChainService : ContractServiceBase, ICrossChainService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public CrossChainService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions) : base(clientService,
        AElfCrossChainConstants.CrossChainSmartContractName)
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<long> GetSyncedHeightByChainId(int chainId)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = chainId == AElfClientConstants.MainChainId
            ? await _clientService.ViewSystemAsync(AElfCrossChainConstants.CrossChainSmartContractName,
                "GetParentChainHeight",
                new Empty(), useClientAlias)
            : await _clientService.ViewSystemAsync(AElfCrossChainConstants.CrossChainSmartContractName,
                "GetSideChainHeight", new Int32Value
                {
                    Value = chainId
                }, useClientAlias);
        var height = new Int64Value();
        height.MergeFrom(result);
        return height.Value;
    }
}