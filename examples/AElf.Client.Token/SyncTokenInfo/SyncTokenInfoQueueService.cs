using Volo.Abp.DependencyInjection;

namespace AElf.Client.Token.SyncTokenInfo;

public class SyncTokenInfoQueueService : ISyncTokenInfoQueueService, ITransientDependency
{
    private readonly ISyncTokenInfoService _syncTokenInfoService;
    private readonly ITaskQueueManager _taskQueueManager;

    public SyncTokenInfoQueueService(ISyncTokenInfoService syncTokenInfoService,
        ITaskQueueManager taskQueueManager)
    {
        _syncTokenInfoService = syncTokenInfoService;
        _taskQueueManager = taskQueueManager;
    }

    public void Enqueue(string symbol)
    {
        _taskQueueManager.Enqueue(async () => { await _syncTokenInfoService.SyncTokenInfoAsync(symbol); },
            AElfTokenConstants.SyncTokenInfoQueueName);
    }
}