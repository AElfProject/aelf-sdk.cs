using AElf.Client.Abp.Token;
using AElf.Client.Abp.Token.SyncTokenInfo;
using AElf.Contracts.NFT;
using AElf.Types;
using Shouldly;
using TransferInput = AElf.Contracts.MultiToken.TransferInput;

namespace AElf.Client.Abp.Test.Token;

[Trait("Category", "TokenContractService")]
public sealed class TokenServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly ITokenService _tokenService;
    private readonly ISyncTokenInfoQueueService _syncTokenInfoQueueService;

    public TokenServiceTests()
    {
        _tokenService = GetRequiredService<ITokenService>();
        _syncTokenInfoQueueService = GetRequiredService<ISyncTokenInfoQueueService>();
    }

    [Theory]
    [InlineData("ELF")]
    public async Task GetTokenInfoTest(string symbol)
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync(symbol);
        tokenInfo.Symbol.ShouldBe(symbol);
    }

    [Theory]
    [InlineData("2nSXrp4iM3A1gB5WKXjkwJQwy56jzcw1ESNpVnWywnyjXFixGc", "ELF", 10_00000000)]
    public async Task TransferTest(string address, string symbol, long amount)
    {
        var result = await _tokenService.TransferAsync(new TransferInput
        {
            To = Address.FromBase58(address),
            Symbol = symbol,
            Amount = amount
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("BA994198147")]
    public async Task SyncTokenInfoTest(string symbol)
    {
        _syncTokenInfoQueueService.Enqueue(symbol);
    }

    [Theory]
    [InlineData("CO429872652")]
    public async Task CrossChainCreateNFTProtocolTest(string symbol)
    {
        await _tokenService.CrossChainCreateNFTProtocolAsync(new CrossChainCreateInput
        {
            Symbol = symbol
        });
    }
}