using AElf.Contracts.Consensus.AEDPoS;

namespace AElf.Client.Consensus.AEDPoS;

public interface IConsensusService
{
    Task<MinerList> GetCurrentMinerList();
}