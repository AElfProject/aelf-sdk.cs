using AElf.Standards.ACS0;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Client.Genesis;

public partial class GenesisService
{
    public async Task<AuthorityInfo> GetContractDeploymentController()
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewAsync<AuthorityInfo>(_contractAddress, "GetContractDeploymentController",
            new Empty(), useClientAlias);
    }

    public async Task<SmartContractRegistration> GetSmartContractRegistrationByCodeHash(Hash codeHash)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewAsync<SmartContractRegistration>(_contractAddress,
            "GetSmartContractRegistrationByCodeHash", codeHash, useClientAlias);
    }

    public async Task<ContractInfo> GetContractInfo(Address contractAddress)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewAsync<ContractInfo>(_contractAddress, "GetContractInfo",
            contractAddress, useClientAlias);
    }
}