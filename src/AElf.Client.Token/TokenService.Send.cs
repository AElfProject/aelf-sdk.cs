using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.Bridge;
using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;
using TransferInput = AElf.Contracts.MultiToken.TransferInput;

namespace AElf.Client.Token;

public partial class TokenService : ContractServiceBase, ITokenService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfContractOptions _contractOptions;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public TokenService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOptions> contractOptions) : base(clientService,
        AElfTokenConstants.TokenSmartContractName)
    {
        _clientService = clientService;
        _contractOptions = contractOptions.Value;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task<SendTransactionResult> CreateTokenAsync(CreateInput createInput)
    {
        var clientAlias = PreferGetUseMainChainClientAlias();
        var tx = await PerformSendTransactionAsync("Create", createInput, clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput)
    {
        var clientAlias = PreferGetUseMainChainClientAlias();
        var tx = await PerformSendTransactionAsync("Create", createInput, clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> MintNFTAsync(MintInput mintInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("Mint", mintInput, clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> ValidateTokenInfoExistsAsync(
        ValidateTokenInfoExistsInput validateTokenInfoExistsInput)
    {
        var clientAlias = PreferGetUseMainChainClientAlias();
        var tx = await PerformSendTransactionAsync("ValidateTokenInfoExists", validateTokenInfoExistsInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainCreateTokenAsync(
        CrossChainCreateTokenInput crossChainCreateTokenInput)
    {
        var clientAlias = PreferGetUseSideChainClientAlias();
        var tx = await PerformSendTransactionAsync("CrossChainCreateToken", crossChainCreateTokenInput, clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainTransferAsync(CrossChainTransferInput crossChainTransferInput,
        string clientAlias)
    {
        var tx = await _clientService.SendSystemAsync(AElfTokenConstants.TokenSmartContractName, "CrossChainTransfer",
            crossChainTransferInput, clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainReceiveTokenAsync(
        CrossChainReceiveTokenInput crossChainReceiveTokenInput, string clientAlias)
    {
        var tx = await PerformSendTransactionAsync("CrossChainReceiveToken", crossChainReceiveTokenInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> CrossChainCreateNFTProtocolAsync(
        CrossChainCreateInput crossChainCreateInput)
    {
        var clientAlias = PreferGetUseSideChainClientAlias();
        ContractAddress = Address.FromBase58(_contractOptions.NFTContractAddress);
        var tx = await PerformSendTransactionAsync("CrossChainCreate", crossChainCreateInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> TransferAsync(TransferInput transferInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("Transfer", transferInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> AddMintersAsync(AddMintersInput addMintersInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        ContractAddress = Address.FromBase58(_contractOptions.NFTContractAddress);
        var tx = await PerformSendTransactionAsync("AddMinters", addMintersInput,
            clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias)
        };
    }

    public async Task<SendTransactionResult> SwapTokenAsync(SwapTokenInput swapTokenInput)
    {
        var clientAlias = _clientConfigOptions.ClientAlias;
        ContractAddress = Address.FromBase58(_contractOptions.BridgeContractAddress);
        var tx = await PerformSendTransactionAsync("SwapToken", swapTokenInput,
            clientAlias);
        var txResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), clientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = txResult
        };
    }

    private string PreferGetUseMainChainClientAlias()
    {
        return !string.IsNullOrEmpty(_clientConfigOptions.MainChainClientAlias)
            ? _clientConfigOptions.MainChainClientAlias
            : _clientConfigOptions.ClientAlias;
    }

    private string PreferGetUseSideChainClientAlias()
    {
        return !string.IsNullOrEmpty(_clientConfigOptions.SideChainClientAlias)
            ? _clientConfigOptions.SideChainClientAlias
            : _clientConfigOptions.ClientAlias;
    }
}