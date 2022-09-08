using AElf.Contracts.Parliament;
using AElf.Standards.ACS3;
using AElf.Types;
using Google.Protobuf;

namespace AElf.Client.Parliament;

public partial class ParliamentService
{
    public async Task<ProposalOutput> CheckProposal(Hash proposalId)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<ProposalOutput>(AElfParliamentConstants.ParliamentSmartContractName,
            "GetProposal", proposalId, useClientAlias);
    }

    public async Task<Organization> GetOrganization(Address organizationAddress)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<Organization>(AElfParliamentConstants.ParliamentSmartContractName,
            "GetOrganization", organizationAddress, useClientAlias);
    }
}