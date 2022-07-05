using AElf.Types;

namespace AElf.Client.Token.CrossChainTransfer;

public interface ICrossChainTransferQueueService
{
    void Enqueue(Address to, string symbol, long amount, string fromClientAlias, string toClientAlias);
}