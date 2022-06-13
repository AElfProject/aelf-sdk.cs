namespace AElf.Client.Abp.Token.SyncTokenInfo;

public interface ISyncTokenInfoService
{
    Task SyncTokenInfoAsync(string symbol);
}