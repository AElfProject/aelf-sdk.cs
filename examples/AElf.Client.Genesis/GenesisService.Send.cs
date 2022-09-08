using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Standards.ACS0;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace AElf.Client.Genesis;

public partial class GenesisService : ContractServiceBase, IGenesisService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly string _contractAddress;

    public GenesisService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions) : base(clientService,
        AElfClientCoreConstants.GenesisSmartContractName)
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = AsyncHelper.RunSync(async () =>
            (await _clientService.GetGenesisContractAddressAsync(_clientConfigOptions.ClientAlias)).ToBase58());
    }

    public async Task<SendTransactionResult> ProposeNewContract(ContractDeploymentInput contractDeploymentInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ProposeNewContract", contractDeploymentInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> ProposeUpdateContract(ContractUpdateInput contractUpdateInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ProposeUpdateContract", contractUpdateInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> ReleaseApprovedContract(ReleaseContractInput releaseContractInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ReleaseApprovedContract", releaseContractInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> ReleaseCodeCheckedContract(ReleaseContractInput releaseContractInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendAsync(_contractAddress, "ReleaseCodeCheckedContract", releaseContractInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }
}