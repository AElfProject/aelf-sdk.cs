using AElf.Types;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Token.CrossChainTransfer;

public class CrossChainTransferQueueService : ICrossChainTransferQueueService, ITransientDependency
{
    private readonly ICrossChainTransferService _crossChainTransferService;
    private readonly ITaskQueueManager _taskQueueManager;

    public CrossChainTransferQueueService(ICrossChainTransferService crossChainTransferService,
        ITaskQueueManager taskQueueManager)
    {
        _crossChainTransferService = crossChainTransferService;
        _taskQueueManager = taskQueueManager;
    }

    public void Enqueue(Address to, string symbol, long amount, string fromClientAlias, string toClientAlias)
    {
        _taskQueueManager.Enqueue(
            async () =>
            {
                await _crossChainTransferService.CrossChainTransferAsync(to, symbol, amount, fromClientAlias,
                    toClientAlias);
            },
            AElfTokenConstants.CrossChainTransferQueueName);
    }
}