using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Parliament;

public partial class ParliamentService : ContractServiceBase, IParliamentService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly IAElfAccountProvider _aelfAccountProvider;

    public ParliamentService(IAElfClientService clientService, IAElfAccountProvider aelfAccountProvider,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
        : base(clientService, AElfParliamentConstants.ParliamentSmartContractName)
    {
        _clientService = clientService;
        _aelfAccountProvider = aelfAccountProvider;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<SendTransactionResult> ApproveAsync(Hash proposalId, string accountAddress)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        _aelfAccountProvider.AddAccountByDefaultPassword(accountAddress);
        var accountAlias = _aelfAccountProvider.GetAliasByAddress(accountAddress);
        var tx = await _clientService.SendSystemAsync(AElfParliamentConstants.ParliamentSmartContractName, "Approve",
            proposalId, useClientAlias, accountAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }
}