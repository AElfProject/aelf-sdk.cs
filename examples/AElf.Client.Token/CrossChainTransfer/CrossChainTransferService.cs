using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.MultiToken;
using AElf.Types;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Token.CrossChainTransfer;

public class CrossChainTransferService : ICrossChainTransferService, ITransientDependency
{
    private readonly ITokenService _tokenService;
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    public ILogger<CrossChainTransferService> Logger { get; set; }

    public CrossChainTransferService(ITokenService tokenService, IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
    {
        _tokenService = tokenService;
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
    }

    public async Task CrossChainTransferAsync(Address to, string symbol, long amount, string fromClientAlias,
        string toClientAlias)
    {
        var fromChainStatus = await _clientService.GetChainStatusAsync(fromClientAlias);
        var toChainStatus = await _clientService.GetChainStatusAsync(toClientAlias);
        var tokenInfo = await _tokenService.GetTokenInfoAsync(symbol);
        var crossChainTransferInput = new CrossChainTransferInput
        {
            To = to,
            Symbol = symbol,
            Amount = amount,
            ToChainId = ChainHelper.ConvertBase58ToChainId(toChainStatus.ChainId),
            IssueChainId = tokenInfo.IssueChainId
        };
        var transferResult = await _tokenService.CrossChainTransferAsync(crossChainTransferInput, fromClientAlias);
        Logger.LogInformation("CrossChainTransfer: {Result}", transferResult.TransactionResult);
        if (transferResult.TransactionResult.Status == TransactionResultStatus.Mined)
        {
            while (true)
            {
                var chainStatus =
                    await _clientService.GetChainStatusAsync(fromClientAlias);
                Logger.LogInformation(
                    "From chain lib height: {LibHeight}, Transfer tx package height: {TransferHeight}",
                    chainStatus.LastIrreversibleBlockHeight, transferResult.TransactionResult.BlockNumber);
                if (chainStatus.LastIrreversibleBlockHeight - transferResult.TransactionResult.BlockNumber > 300)
                    break;
                await Task.Delay(AElfTokenConstants.TenSeconds);
            }

            var merklePath = await _clientService.GetMerklePathByTransactionIdAsync(
                transferResult.TransactionResult.TransactionId.ToHex(),
                fromClientAlias);
            var crossChainReceiveTokenInput = new CrossChainReceiveTokenInput
            {
                FromChainId = ChainHelper.ConvertBase58ToChainId(fromChainStatus.ChainId),
                MerklePath = merklePath,
                ParentChainHeight = transferResult.TransactionResult.BlockNumber,
                TransferTransactionBytes =
                    ByteString.CopyFrom(
                        ByteArrayHelper.HexStringToByteArray(transferResult.Transaction.ToByteArray().ToHex())),
            };

            var crossChainReceiveTokenResult =
                await _tokenService.CrossChainReceiveTokenAsync(crossChainReceiveTokenInput, toClientAlias);
            Logger.LogInformation("CrossChainReceiveToken: {Result}", crossChainReceiveTokenResult.TransactionResult);
        }
    }
}