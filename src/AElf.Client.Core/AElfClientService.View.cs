using AElf.Client.Dto;
using AElf.Client.Core.Options;
using AElf.Client.Services;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace AElf.Client.Core;

public partial class AElfClientService : IAElfClientService, ITransientDependency
{
    private readonly IAElfClientProvider _aelfClientProvider;
    private readonly IAElfAccountProvider _aelfAccountProvider;
    private readonly IObjectMapper<AElfClientModule> _objectMapper;
    private readonly AElfClientConfigOptions _clientConfigOptions;

    public ILogger<AElfClientService> Logger { get; set; }

    public AElfClientService(IAElfClientProvider aelfClientProvider, IAElfAccountProvider aelfAccountProvider,
        IObjectMapper<AElfClientModule> objectMapper, IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions)
    {
        _aelfClientProvider = aelfClientProvider;
        _aelfAccountProvider = aelfAccountProvider;
        _objectMapper = objectMapper;
        _clientConfigOptions = clientConfigOptions.Value;

        Logger = NullLogger<AElfClientService>.Instance;
    }

    public async Task<T> ViewAsync<T>(string contractAddress, string methodName, IMessage parameter, string clientAlias,
        string accountAlias = "Default") where T : IMessage, new()
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: accountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseContract(contractAddress)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        var returnValue = await PerformViewAsync(aelfClient, tx);
        var result = new T();
        result.MergeFrom(returnValue);
        return result;
    }


    public async Task<T> ViewSystemAsync<T>(string systemContractName, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default") where T : IMessage, new()
    {
        if (!systemContractName.StartsWith("AElf.ContractNames."))
        {
            throw new ArgumentException("Invalid system contract name.");
        }

        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: accountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseSystemContract(systemContractName)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        var returnValue = await PerformViewAsync(aelfClient, tx);
        var result = new T();
        result.MergeFrom(returnValue);
        return result;
    }

    public async Task<Address> GetGenesisContractAddressAsync(string clientAlias)
    {
        var chainStatus = await GetChainStatusAsync(clientAlias);
        return Address.FromBase58(chainStatus.GenesisContractAddress);
    }

    private async Task<byte[]> PerformViewAsync(ITransactionAppService aelfClient, Transaction tx)
    {
        var result = await aelfClient.ExecuteTransactionAsync(new ExecuteTransactionDto
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });
        return ByteArrayHelper.HexStringToByteArray(result);
    }
}