using System.Threading.Tasks;
using AElf.Client.Genesis;
using AElf.Client.Test;
using Shouldly;

namespace AElf.Client.Abp.Test.Genesis;

[Trait("Category", "GenesisContractService")]

public sealed class GenesisServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IGenesisService _genesisService;
    private readonly IDeployContractService _deployService;

    public GenesisServiceTests()
    {
        _genesisService = GetRequiredService<IGenesisService>();
        _deployService = GetRequiredService<IDeployContractService>();
    }

    [Theory]
    [InlineData("AElf.Contracts.NFT")]
    public async Task DeployContract(string contractName)
    {
        var address = await _deployService.DeployContract(contractName);
        address.ShouldNotBeNull();
    }
}