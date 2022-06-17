using AElf.Client.Abp.Genesis;
using AElf.Client.Abp.Genesis.DeployContract;
using Shouldly;

namespace AElf.Client.Abp.Test.Genesis;

[Trait("Category", "GenesisContractService")]

public sealed class GenesisServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IGenesisService _genesisService;
    private readonly IDeployContractService _deploy;
    
    public GenesisServiceTests()
    {
        _genesisService = GetRequiredService<IGenesisService>();
        _deploy = GetRequiredService<IDeployContractService>();
    }

    [Theory]
    [InlineData("AElf.Contracts.NFT")]
    public async Task DeployContract(string contractName)
    {
        var address = await _deploy.DeployContract(contractName);
        address.ShouldNotBeNull();
    }
}