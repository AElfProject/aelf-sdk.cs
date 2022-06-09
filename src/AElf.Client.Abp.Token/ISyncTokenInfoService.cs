namespace AElf.Client.Abp.Token;

public interface ISyncTokenInfoService
{
    Task SyncTokenInfoAsync(string symbol);
}