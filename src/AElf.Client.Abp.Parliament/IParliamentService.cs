using AElf.Types;
using AElf.Standards.ACS3;
using AElf.Contracts.Parliament;

namespace AElf.Client.Abp.Parliament;

public interface IParliamentService
{
    Task<SendTransactionResult> ApproveAsync(Hash proposalId, string? accountAlias, string accountAddress);
    
    Task<ProposalOutput> CheckProposal(Hash proposalId);

    Task<Organization> GetOrganization(Address organizationAddress);

}