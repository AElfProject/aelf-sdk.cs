using AElf.Contracts.Consensus.AEDPoS;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Client.Consensus.AEDPoS;

public partial class ConsensusService
{
    public async Task<MinerList> GetCurrentMinerList()
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<MinerList>(AElfConsensusConstants.ConsensusSmartContractName,
            "GetCurrentMinerList", new Empty(), useClientAlias);
    }
}