using AElf.Client.Options;
using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;
using TransferInput = AElf.Contracts.MultiToken.TransferInput;

namespace AElf.Client.Abp.Token;

public partial class TokenService : ContractServiceBase, ITokenService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public TokenService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
        : base(clientService, AElfTokenConstants.TokenSmartContractName)
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<SendTransactionResult> CreateTokenAsync(CreateInput createInput)
    {
        var useClientAlias = PreferGetUseMainChainClientAlias();
        var tx = await PerformSendTransactionAsync("Create", createInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput)
    {
        var useClientAlias = PreferGetUseMainChainClientAlias();
        var tx = await PerformSendTransactionAsync("Create", createInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> MintNFTAsync(MintInput mintInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("Mint", mintInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> ValidateTokenInfoExistsAsync(
        ValidateTokenInfoExistsInput validateTokenInfoExistsInput)
    {
        var useClientAlias = PreferGetUseMainChainClientAlias();
        var tx = await PerformSendTransactionAsync("ValidateTokenInfoExists", validateTokenInfoExistsInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainCreateTokenAsync(
        CrossChainCreateTokenInput crossChainCreateTokenInput)
    {
        var useClientAlias = PreferGetUseSidechainClientAlias();
        var tx = await PerformSendTransactionAsync("CrossChainCreateToken", crossChainCreateTokenInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainTransferAsync(CrossChainTransferInput crossChainTransferInput,
        string useClientAlias)
    {
        var tx = await _clientService.SendSystemAsync(AElfTokenConstants.TokenSmartContractName, "CrossChainTransfer",
            crossChainTransferInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainReceiveTokenAsync(
        CrossChainReceiveTokenInput crossChainReceiveTokenInput, string useClientAlias)
    {
        var tx = await PerformSendTransactionAsync("CrossChainReceiveToken", crossChainReceiveTokenInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> TransferAsync(TransferInput transferInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("Transfer", transferInput,
            useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    private string PreferGetUseMainChainClientAlias()
    {
        return !string.IsNullOrEmpty(_clientConfigOptions.MainChainClientAlias)
            ? _clientConfigOptions.MainChainClientAlias
            : _clientConfigOptions.ClientAlias;
    }

    private string PreferGetUseSidechainClientAlias()
    {
        return !string.IsNullOrEmpty(_clientConfigOptions.SidechainClientAlias)
            ? _clientConfigOptions.SidechainClientAlias
            : _clientConfigOptions.ClientAlias;
    }
}