using AElf.Client.Options;
using AElf.Standards.ACS0;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Abp.Genesis;

public partial class GenesisService : ContractServiceBase, IGenesisService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly string _contractAddress;

    public GenesisService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOption> contractOptions) : base(clientService, 
        Address.FromBase58(contractOptions.Value.GenesisContractAddress))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = contractOptions.Value.GenesisContractAddress;
    }

    public async Task<SendTransactionResult> ProposeNewContract(ContractDeploymentInput contractDeploymentInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ProposeNewContract", contractDeploymentInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> ProposeUpdateContract(ContractUpdateInput contractUpdateInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ProposeUpdateContract", contractUpdateInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> ReleaseApprovedContract(ReleaseContractInput releaseContractInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ReleaseApprovedContract", releaseContractInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> ReleaseCodeCheckedContract(ReleaseContractInput releaseContractInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ReleaseCodeCheckedContract", releaseContractInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }
}