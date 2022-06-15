using AElf.Client.Options;
using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;
using TransferInput = AElf.Contracts.MultiToken.TransferInput;

namespace AElf.Client.Abp.Token;

public partial class TokenService : ContractServiceBase, ITokenService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfTokenOptions _tokenOptions;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public TokenService(IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfTokenOptions> tokenOptions) : base(clientService,
        AElfTokenConstants.TokenSmartContractName)
    {
        _clientService = clientService;
        _tokenOptions = tokenOptions.Value;
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
        var clientAlias = PreferGetUseSidechainClientAlias();
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
        var clientAlias = PreferGetUseSidechainClientAlias();
        ContractAddress = Address.FromBase58(_tokenOptions.NFTContractAddress);
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