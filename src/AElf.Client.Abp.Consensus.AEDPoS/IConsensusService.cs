using AElf.Contracts.Consensus.AEDPoS;

namespace AElf.Client.Abp.Consensus.AEDPoS;

public interface IConsensusService
{
    Task<MinerList> GetCurrentMinerList();
}