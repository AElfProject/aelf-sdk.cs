namespace AElf.Client.Token.SyncTokenInfo;

public interface ISyncTokenInfoQueueService
{
    void Enqueue(string symbol);
}