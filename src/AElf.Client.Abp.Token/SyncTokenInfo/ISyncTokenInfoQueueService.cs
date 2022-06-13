namespace AElf.Client.Abp.Token.SyncTokenInfo;

public interface ISyncTokenInfoQueueService
{
    void Enqueue(string symbol);
}