namespace AElf.Client.Abp.CrossChain;

public interface ICrossChainService
{
    Task<long> GetSyncedHeightByChainId(int chainId);
}