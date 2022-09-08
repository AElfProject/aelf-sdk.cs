namespace AElf.Client.CrossChain;

public interface ICrossChainService
{
    Task<long> GetSyncedHeightByChainId(int chainId);
}