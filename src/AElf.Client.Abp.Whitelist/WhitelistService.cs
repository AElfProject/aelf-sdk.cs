using AElf.Client.Options;
using AElf.Contracts.Whitelist;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Abp.Whitelist;

public class WhitelistService : ContractServiceBase, IWhitelistService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    protected WhitelistService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions) : base(clientService,
        Address.FromBase58("aceGtyU2fVcBkViZcaqZXHHjd7eNAJ6NPwbuFwhqv6He49BS1"))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<SendTransactionResult> CreateWhitelistAsync(CreateWhitelistInput createWhitelistInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("CreateWhitelist", createWhitelistInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }
}