using AElf.Client.Abp.Token;
using Shouldly;

namespace AElf.Client.Abp.Test.Token;

[Trait("Category", "TokenContractService")]
public sealed class TokenServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly ITokenService _tokenService;

    public TokenServiceTests()
    {
        _tokenService = GetRequiredService<ITokenService>();
    }

    [Theory]
    [InlineData("ELF")]
    public async Task GetTokenInfoTest(string symbol)
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync(symbol);
        tokenInfo.Symbol.ShouldBe(symbol);
    }
}