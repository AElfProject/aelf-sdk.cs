using AElf.Client.Options;
using AElf.Contracts.NFT;
using AElf.Types;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;

namespace AElf.Client.Abp.TokenManager;

public partial class TokenService : ITokenService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly TokenManagerOptions _tokenManagerOptions;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public TokenService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<TokenManagerOptions> tokenManagerOptions)
    {
        _clientService = clientService;
        _tokenManagerOptions = tokenManagerOptions.Value;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<TransactionResult> CreateTokenAsync(CreateInput createInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var txId = await _clientService.SendSystemAsync(TokenManagerConstants.TokenSmartContractName, "Create", createInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(txId, useClientAlias);
        return txResult;
    }

    public async Task<TransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var txId = await _clientService.SendAsync(_tokenManagerOptions.NFTContractAddress, "Create", createInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(txId, useClientAlias);
        return txResult;
    }

    public async Task<TransactionResult> MintNFTAsync(MintInput mintInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var txId = await _clientService.SendAsync(_tokenManagerOptions.NFTContractAddress, "Mint", mintInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(txId, useClientAlias);
        return txResult;
    }

    public async Task<TransactionResult> SyncTokenInfoAsync(string symbol)
    {
        throw new NotImplementedException();
    }
}