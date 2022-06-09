namespace AElf.Client.Abp.Token;

public interface ISyncTokenInfoQueueService
{
    void Enqueue(string symbol);
}