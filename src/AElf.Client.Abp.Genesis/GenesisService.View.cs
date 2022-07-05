using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Client.Abp.Genesis;

public partial class GenesisService
{
    public async Task<AuthorityInfo> GetContractDeploymentController()
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetContractDeploymentController",
            new Empty(), useClientAlias); 
        var authorityInfo = new AuthorityInfo();
        authorityInfo.MergeFrom(result);
        return authorityInfo;
    }

    public async Task<SmartContractRegistration> GetSmartContractRegistrationByCodeHash(Hash codeHash)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetSmartContractRegistrationByCodeHash",
            codeHash , useClientAlias); 
        var smartContractRegistration = new SmartContractRegistration();
        smartContractRegistration.MergeFrom(result);
        return smartContractRegistration;
    }
}