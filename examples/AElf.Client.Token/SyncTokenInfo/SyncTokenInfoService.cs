using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Client.CrossChain;
using AElf.Contracts.MultiToken;
using AElf.Types;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Token.SyncTokenInfo;

public class SyncTokenInfoService : ISyncTokenInfoService, ITransientDependency
{
    private readonly ITokenService _tokenService;
    private readonly ICrossChainService _crossChainService;
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    public ILogger<SyncTokenInfoService> Logger { get; set; }

    public SyncTokenInfoService(ITokenService tokenService, ICrossChainService crossChainService,
        IAElfClientService clientService, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
    {
        _tokenService = tokenService;
        _crossChainService = crossChainService;
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;

        Logger = NullLogger<SyncTokenInfoService>.Instance;
    }

    public async Task SyncTokenInfoAsync(string symbol)
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync(symbol);
        var validateInput = new ValidateTokenInfoExistsInput
        {
            Symbol = tokenInfo.Symbol,
            TokenName = tokenInfo.TokenName,
            Decimals = tokenInfo.Decimals,
            IsBurnable = tokenInfo.IsBurnable,
            IssueChainId = tokenInfo.IssueChainId,
            Issuer = tokenInfo.Issuer,
            TotalSupply = tokenInfo.TotalSupply,
            ExternalInfo = { tokenInfo.ExternalInfo.Value }
        };

        var validateResult = await _tokenService.ValidateTokenInfoExistsAsync(validateInput);
        var packagedBlockHeight = validateResult.TransactionResult.BlockNumber;
        Logger.LogInformation("ValidateTokenInfoExists: {Result}", validateResult.TransactionResult);
        if (validateResult.TransactionResult.Status == TransactionResultStatus.Mined)
        {
            while (true)
            {
                var syncedMainChainHeight =
                    await _crossChainService.GetSyncedHeightByChainId(AElfClientConstants.MainChainId);
                Logger.LogInformation(
                    "Synced main chain height: {MainChainHeight}, Validate tx package height: {ValidateHeight}",
                    syncedMainChainHeight, validateResult.TransactionResult.BlockNumber);
                if (syncedMainChainHeight >= packagedBlockHeight)
                {
                    break;
                }

                await Task.Delay(AElfTokenConstants.TenSeconds);
            }

            var merklePath = await _clientService.GetMerklePathByTransactionIdAsync(
                validateResult.TransactionResult.TransactionId.ToHex(),
                _clientConfigOptions.MainChainClientAlias);
            var crossChainCreateTokenInput = new CrossChainCreateTokenInput
            {
                FromChainId = AElfClientConstants.MainChainId,
                ParentChainHeight = validateResult.TransactionResult.BlockNumber,
                TransactionBytes =
                    ByteString.CopyFrom(
                        ByteArrayHelper.HexStringToByteArray(validateResult.Transaction.ToByteArray().ToHex())),
                MerklePath = merklePath
            };

            var crossChainCreateTokenResult =
                await _tokenService.CrossChainCreateTokenAsync(crossChainCreateTokenInput);
            Logger.LogInformation("CrossChainCreateToken: {Result}", crossChainCreateTokenResult.TransactionResult);
        }
    }
}