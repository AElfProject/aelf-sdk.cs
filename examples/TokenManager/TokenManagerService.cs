using AElf.Client;
using AElf.Client.Token;
using AElf.Client.Token.CrossChainTransfer;
using AElf.Client.Token.SyncTokenInfo;
using AElf.Contracts.MultiToken;
using AElf.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace TokenManager;

public class TokenManagerService : ITransientDependency
{
    private readonly ITokenService _tokenService;
    private readonly ISyncTokenInfoQueueService _syncTokenInfoQueueService;
    private readonly ICrossChainTransferQueueService _crossChainTransferQueueService;
    private IServiceScopeFactory ServiceScopeFactory { get; }

    public ILogger<TokenManagerService> Logger { get; set; }

    public TokenManagerService(IServiceScopeFactory serviceScopeFactory,
        ITokenService tokenService,
        ISyncTokenInfoQueueService syncTokenInfoQueueService,
        ICrossChainTransferQueueService crossChainTransferQueueService)
    {
        _tokenService = tokenService;
        _syncTokenInfoQueueService = syncTokenInfoQueueService;
        _crossChainTransferQueueService = crossChainTransferQueueService;
        ServiceScopeFactory = serviceScopeFactory;

        Logger = NullLogger<TokenManagerService>.Instance;
    }

    public async Task GetTokenInfoAsync(string symbol)
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync(symbol);
        Logger.LogInformation("{TokenInfo}", tokenInfo.ToString());
    }

    public async Task SyncTokenInfoAsync(string symbol)
    {
        _syncTokenInfoQueueService.Enqueue(symbol);
        Logger.LogInformation("Enqueued SyncTokenInfo");
    }

    public async Task CrossChainTransferAsync(Address to, string symbol, long amount, string toClientAlias)
    {
        _crossChainTransferQueueService.Enqueue(
            to, symbol, amount, EndpointType.TestNetMainChain.ToString(), toClientAlias);
        Logger.LogInformation("Enqueued CrossChainTransfer");
    }

    public async Task TransferAsync(Address to, string symbol, long amount)
    {
        await _tokenService.TransferAsync(new TransferInput
        {
            To = to,
            Symbol = symbol,
            Amount = amount,
        });
    }

    public async Task<long> GetBalanceAsync(Address owner, string symbol)
    {
        return (await _tokenService.GetTokenBalanceAsync(symbol, owner)).Balance;
    }
}