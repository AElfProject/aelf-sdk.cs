using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AElf.Client.Genesis;
using AElf.Client.Test;
using AElf.Types;
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

    [Theory]
    [InlineData("UoHeeCXZ6fV481oD3NXASSexWVtsPLgv2Wthm3BGrPAgqdS5d")]
    public async Task GetContractInfo(string contractAddress)
    {
        var address = Address.FromBase58(contractAddress);
        var contractInfo = await _genesisService.GetContractInfo(address);
        contractInfo.Author.ToBase58().ShouldBe("2UKQnHcQvhBT6X6ULtfnuh3b9PVRvVMEroHHkcK4YfcoH1Z1x2");
    }
}