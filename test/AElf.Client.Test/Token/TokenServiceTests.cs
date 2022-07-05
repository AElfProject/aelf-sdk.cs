using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Token;
using AElf.Client.Token.SyncTokenInfo;
using AElf.Contracts.Bridge;
using AElf.Contracts.NFT;
using AElf.Types;
using Google.Protobuf;
using Shouldly;
using TransferInput = AElf.Contracts.MultiToken.TransferInput;

namespace AElf.Client.Test.Token;

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
    [InlineData("2nSXrp4iM3A1gB5WKXjkwJQwy56jzcw1ESNpVnWywnyjXFixGc", "ELF", 1_00000000)]
    public async Task TransferTest(string address, string symbol, long amount)
    {
        var result = await _tokenService.TransferAsync(new TransferInput
        {
            To = Address.FromBase58(address),
            Symbol = symbol,
            Amount = amount
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var logEvent = result.TransactionResult.Logs.First(l => l.Name == nameof(Contracts.MultiToken.Transferred));
        var transferred = new Contracts.MultiToken.Transferred();
        foreach (var indexed in logEvent.Indexed)
        {
            transferred.MergeFrom(indexed);
        }

        transferred.MergeFrom(logEvent.NonIndexed);
        transferred.Symbol.ShouldBe(symbol);
        transferred.To.ToBase58().ShouldBe(address);
        transferred.Amount.ShouldBe(amount);
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

    [Theory]
    [InlineData("BA417054001", "JQkVTWz5HXxEmNXzTtsAVHC7EUTeiFktzoFUu9TyA6MWngkem")]
    public async Task AddMintersTest(string symbol, string addressBase58)
    {
        var address = Address.FromBase58(addressBase58);
        await _tokenService.AddMintersAsync(new AddMintersInput
        {
            Symbol = symbol,
            MinterList = new MinterList
            {
                Value = { address }
            }
        });
    }

    [Theory]
    [InlineData("bb16f381b0f2e795a988285dec3a68affacdccd7d3ac2e74edc808c102efcd95", 228, "9413000000000000000000")]
    public async Task SwapTokenTest(string swapIdHex, long receiptId, string amount)
    {
        var swapId = Hash.LoadFromHex(swapIdHex);
        await _tokenService.SwapTokenAsync(new SwapTokenInput
        {
            SwapId = swapId,
            OriginAmount = amount,
            ReceiptId = receiptId
        });
    }
}