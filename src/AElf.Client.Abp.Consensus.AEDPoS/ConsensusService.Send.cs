using AElf.Client.Options;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Abp.Consensus.AEDPoS;

public partial class ConsensusService : ContractServiceBase, IConsensusService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public ConsensusService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
        : base(clientService, AElfConsensusConstants.ConsensusSmartContractName)
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
    }
}