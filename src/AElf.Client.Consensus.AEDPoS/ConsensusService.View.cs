using AElf.Contracts.Consensus.AEDPoS;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Client.Consensus.AEDPoS;

public partial class ConsensusService
{
    public async Task<MinerList> GetCurrentMinerList()
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewSystemAsync(AElfConsensusConstants.ConsensusSmartContractName, "GetCurrentMinerList",
            new Empty(), useClientAlias);
        var minerList = new MinerList();
        minerList.MergeFrom(result);
        return minerList;
    }
}