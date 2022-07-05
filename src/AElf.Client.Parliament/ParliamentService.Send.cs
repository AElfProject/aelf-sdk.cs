using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Standards.ACS3;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Parliament;

public partial class ParliamentService : ContractServiceBase, IParliamentService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public ParliamentService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
        : base(clientService, AElfParliamentConstants.ParliamentSmartContractName)
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<SendTransactionResult> ApproveAsync(Hash proposalId, string? accountAlias, string address)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await _clientService.SendSystemAsync(AElfParliamentConstants.ParliamentSmartContractName, "Approve",
            proposalId, useClientAlias, accountAlias, address);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }
}