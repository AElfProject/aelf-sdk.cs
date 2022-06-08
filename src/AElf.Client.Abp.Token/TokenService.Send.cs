using AElf.Client.Options;
using AElf.Contracts.NFT;
using AElf.Types;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;

namespace AElf.Client.Abp.Token;

public partial class TokenService : ITokenService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfTokenOptions _aElfTokenOptions;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public TokenService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfTokenOptions> tokenManagerOptions)
    {
        _clientService = clientService;
        _aElfTokenOptions = tokenManagerOptions.Value;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<TransactionResult> CreateTokenAsync(CreateInput createInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var txId = await _clientService.SendSystemAsync(AElfTokenConstants.TokenSmartContractName, "Create", createInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(txId, useClientAlias);
        return txResult;
    }

    public async Task<TransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var txId = await _clientService.SendAsync(_aElfTokenOptions.NFTContractAddress, "Create", createInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(txId, useClientAlias);
        return txResult;
    }

    public async Task<TransactionResult> MintNFTAsync(MintInput mintInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var txId = await _clientService.SendAsync(_aElfTokenOptions.NFTContractAddress, "Mint", mintInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(txId, useClientAlias);
        return txResult;
    }

    public async Task<TransactionResult> SyncTokenInfoAsync(string symbol)
    {
        throw new NotImplementedException();
    }
}