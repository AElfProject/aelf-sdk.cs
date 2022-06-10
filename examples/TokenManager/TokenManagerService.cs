using AElf.Client.Abp;
using AElf.Client.Abp.Token;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace TokenManager;

public class TokenManagerService : ITransientDependency
{
    private readonly ITokenService _tokenService;
    private readonly ISyncTokenInfoQueueService _syncTokenInfoQueueService;
    private readonly IObjectMapper<AElfClientModule> _objectMapper;
    private readonly SyncTokenInfoOptions _syncTokenInfoOptions;
    private IServiceScopeFactory ServiceScopeFactory { get; }

    public ILogger<TokenManagerService> Logger { get; set; }

    public TokenManagerService(IServiceScopeFactory serviceScopeFactory,
        ITokenService tokenService,
        ISyncTokenInfoQueueService syncTokenInfoQueueService,
        IObjectMapper<AElfClientModule> objectMapper,
        IOptionsSnapshot<SyncTokenInfoOptions> syncTokenInfoOptions)
    {
        _tokenService = tokenService;
        _syncTokenInfoQueueService = syncTokenInfoQueueService;
        _objectMapper = objectMapper;
        _syncTokenInfoOptions = syncTokenInfoOptions.Value;
        ServiceScopeFactory = serviceScopeFactory;

        Logger = NullLogger<TokenManagerService>.Instance;
    }

    public async Task GetTokenInfoAsync()
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync("ELF");
        Logger.LogInformation("{TokenInfo}", tokenInfo.ToString());
    }

    public async Task SyncTokenInfoAsync()
    {
        _syncTokenInfoQueueService.Enqueue("JUNETEN");
        Logger.LogInformation("Enqueued");
    }
}