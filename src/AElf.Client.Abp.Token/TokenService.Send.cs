using AElf.Client.Options;
using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;

namespace AElf.Client.Abp.Token;

public partial class TokenService : ITokenService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfTokenOptions _tokenOptions;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public TokenService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfTokenOptions> tokenManagerOptions)
    {
        _clientService = clientService;
        _tokenOptions = tokenManagerOptions.Value;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<SendTransactionResult> CreateTokenAsync(CreateInput createInput)
    {
        var useClientAlias = PreferGetUseMainChainClientAlias();
        var tx = await _clientService.SendSystemAsync(AElfTokenConstants.TokenSmartContractName, "Create",
            createInput, useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = txResult
        };
    }

    public async Task<SendTransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput)
    {
        var useClientAlias = PreferGetUseMainChainClientAlias();
        var tx = await _clientService.SendAsync(_tokenOptions.NFTContractAddress, "Create", createInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = txResult
        };
    }

    public async Task<SendTransactionResult> MintNFTAsync(MintInput mintInput)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var tx = await _clientService.SendAsync(_tokenOptions.NFTContractAddress, "Mint", mintInput,
            useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = txResult
        };
    }

    public async Task<SendTransactionResult> ValidateTokenInfoExistsAsync(
        ValidateTokenInfoExistsInput validateTokenInfoExistsInput)
    {
        var useClientAlias = PreferGetUseMainChainClientAlias();
        var tx = await _clientService.SendSystemAsync(AElfTokenConstants.TokenSmartContractName,
            "ValidateTokenInfoExists", validateTokenInfoExistsInput, useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = txResult
        };
    }

    public async Task<SendTransactionResult> CrossChainCreateTokenAsync(
        CrossChainCreateTokenInput crossChainCreateTokenInput)
    {
        var useClientAlias = PreferGetUseSidechainClientAlias();
        var tx = await _clientService.SendSystemAsync(AElfTokenConstants.TokenSmartContractName,
            "CrossChainCreateToken", crossChainCreateTokenInput, useClientAlias);
        var txResult = await _clientService.GetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = txResult
        };
    }

    private string PreferGetUseMainChainClientAlias()
    {
        return string.IsNullOrEmpty(_clientConfigOptions.UseMainChainClientAlias)
            ? _clientConfigOptions.UseMainChainClientAlias
            : _clientConfigOptions.UseClientAlias;
    }

    private string PreferGetUseSidechainClientAlias()
    {
        return string.IsNullOrEmpty(_clientConfigOptions.UseSidechainClientAlias)
            ? _clientConfigOptions.UseSidechainClientAlias
            : _clientConfigOptions.UseClientAlias;
    }
}