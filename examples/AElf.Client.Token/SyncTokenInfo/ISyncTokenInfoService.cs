namespace AElf.Client.Token.SyncTokenInfo;

public interface ISyncTokenInfoService
{
    Task SyncTokenInfoAsync(string symbol);
}