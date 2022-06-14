using AElf.Contracts.Whitelist;

namespace AElf.Client.Abp.Test.Whitelist;

[Trait("Category", "WhitelistContractService")]
public class WhitelistServiceTests : AElfClientAbpContractServiceTestBase
{
    [Theory]
    [InlineData(true)]
    public async Task CreateWhitelistTest(bool isClonable)
    {
        var extraInfoList = new ExtraInfoList();
        
    }
}