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
        var result = await _clientService.ViewSystemAsync(AElfParliamentConstants.ParliamentSmartContractName, "GetProposal",
            proposalId, useClientAlias);
        var proposalOutput = new ProposalOutput();
        proposalOutput.MergeFrom(result);
        return proposalOutput;
    }

    public async Task<Organization> GetOrganization(Address organizationAddress)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewSystemAsync(AElfParliamentConstants.ParliamentSmartContractName, "GetOrganization",
            organizationAddress, useClientAlias);
        var organization = new Organization();
        organization.MergeFrom(result);
        return organization;
    }
}