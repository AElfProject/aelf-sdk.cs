using AElf.Client;
using AElf.Client.Abp;
using AElf.Client.Abp.TokenManager;
using AElf.Client.Dto;
using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Serilog;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace TokenManager;

public class TokenManagerService : ITransientDependency
{
    private readonly ITokenService _tokenService;
    private readonly IObjectMapper<AElfClientModule> _objectMapper;
    private readonly SyncTokenInfoOptions _syncTokenInfoOptions;
    private IServiceScopeFactory ServiceScopeFactory { get; }

    public ILogger<TokenManagerService> Logger { get; set; }

    public TokenManagerService(IServiceScopeFactory serviceScopeFactory,
        ITokenService tokenService,
        IObjectMapper<AElfClientModule> objectMapper,
        IOptionsSnapshot<SyncTokenInfoOptions> syncTokenInfoOptions)
    {
        _tokenService = tokenService;
        _objectMapper = objectMapper;
        _syncTokenInfoOptions = syncTokenInfoOptions.Value;
        ServiceScopeFactory = serviceScopeFactory;

        Logger = NullLogger<TokenManagerService>.Instance;
    }

    public async Task GetTokenInfoAsync()
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync("ELF");
        Logger.LogInformation("{TokenInfo}", tokenInfo.ToString());
    }

    private async Task ValidateTokenInfoExistsAsync()
    {
        var client = new AElfClientBuilder().UsePublicEndpoint(EndpointType.TestNetMainChain).Build();
        var protocolCreateEvent = await GetNFTProtocolCreatedEventAsync(client);

        var validateTokenInfoExistsInput = new ValidateTokenInfoExistsInput
        {
            Symbol = protocolCreateEvent.Symbol,
            TokenName = protocolCreateEvent.ProtocolName,
            Decimals = 0,
            IsBurnable = protocolCreateEvent.IsBurnable,
            Issuer = protocolCreateEvent.Creator,
            IssueChainId = protocolCreateEvent.IssueChainId,
            TotalSupply = protocolCreateEvent.TotalSupply,
            ExternalInfo = {protocolCreateEvent.Metadata.Value}
        };

        using var scope = ServiceScopeFactory.CreateScope();
        var clientService = scope.ServiceProvider.GetRequiredService<IAElfClientService>();
        var txId = await clientService.SendSystemAsync(TokenManagerConstants.TokenSmartContractName,
            "ValidateTokenInfoExists", validateTokenInfoExistsInput, EndpointType.TestNetMainChain.ToString(),
            "eanz");
        Logger.LogInformation(txId);
        var result = await client.GetTransactionResultAsync(txId);
        Logger.LogInformation(result.Status);
        Logger.LogInformation(result.Error);
    }

    public async Task SyncTokenInfoAsync()
    {
        var client = new AElfClientBuilder().UsePublicEndpoint(EndpointType.TestNetMainChain).Build();
        var txId = _syncTokenInfoOptions.ValidateTokenInfoExistsTransactionId;
        var txResult = await client.GetTransactionResultAsync(txId);
        var merklePath = await client.GetMerklePathByTransactionIdAsync(txId);

        using var scope = ServiceScopeFactory.CreateScope();
        var clientService = scope.ServiceProvider.GetRequiredService<IAElfClientService>();
        var tx = _objectMapper.Map<TransactionDto, Transaction>(txResult.Transaction);
        var crossChainCreateTokenInput = new CrossChainCreateTokenInput
        {
            FromChainId = AElfClientConstants.MainChainId,
            MerklePath = new MerklePath
            {
                MerklePathNodes =
                {
                    merklePath?.MerklePathNodes.Select(n => new MerklePathNode
                    {
                        Hash = Hash.LoadFromHex(n.Hash),
                        IsLeftChildNode = n.IsLeftChildNode
                    })
                }
            },
            ParentChainHeight = txResult.BlockNumber,
            TransactionBytes = tx.ToByteString()
        };
        var resultTxId = await clientService.SendAsync("7RzVGiuVWkvL4VfVHdZfQF2Tri3sgLe9U991bohHFfSRZXuGX", "CrossChainCreateToken",
            crossChainCreateTokenInput, EndpointType.TestNetSidechain.ToString(), "eanz");

        Logger.LogInformation(resultTxId);
        var result = await client.GetTransactionResultAsync(resultTxId);
        Logger.LogInformation(result.Status);
        Logger.LogInformation(result.Error);
    }

    private async Task<NFTProtocolCreated> GetNFTProtocolCreatedEventAsync(AElfClient client)
    {
        var transactionResultDto = await client.GetTransactionResultAsync(_syncTokenInfoOptions.CreateTransactionId);
        var protocolCreated = new NFTProtocolCreated();
        var logEvent = ByteString.FromBase64(transactionResultDto.Logs.Single(l => l.Name == nameof(NFTProtocolCreated))
            .NonIndexed);
        protocolCreated.MergeFrom(logEvent);
        Logger.LogInformation(protocolCreated.ToString());
        return protocolCreated;
    }
}