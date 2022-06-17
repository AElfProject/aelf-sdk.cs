using AElf.Standards.ACS0;
using AElf.Types;

namespace AElf.Client.Abp.Genesis;

public interface IGenesisService
{
    Task<SendTransactionResult> ProposeNewContract(ContractDeploymentInput contractDeploymentInput);
    
    Task<SendTransactionResult> ProposeUpdateContract(ContractUpdateInput contractUpdateInput);

    Task<SendTransactionResult> ReleaseApprovedContract(ReleaseContractInput releaseContractInput);

    Task<SendTransactionResult> ReleaseCodeCheckedContract(ReleaseContractInput releaseContractInput);

    
    Task<AuthorityInfo> GetContractDeploymentController();
    Task<SmartContractRegistration> GetSmartContractRegistrationByCodeHash(Hash codeHash);
}