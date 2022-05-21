using AElf.Client.Dto;
using AElf.Client.Options;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client;

public class AElfClientService : IAElfClientService, ITransientDependency
{
    private readonly IAElfClientProvider _aelfClientProvider;
    private readonly IAElfAccountProvider _aelfAccountProvider;
    private readonly AElfClientOptions _aelfClientOptions;
    private readonly AElfAccountOptions _aelfAccountOptions;

    public AElfClientService(IAElfClientProvider aelfClientProvider, IAElfAccountProvider aelfAccountProvider,
        IOptionsSnapshot<AElfClientOptions> aelfClientOptions, IOptionsSnapshot<AElfAccountOptions> aelfAccountOptions)
    {
        _aelfClientProvider = aelfClientProvider;
        _aelfAccountProvider = aelfAccountProvider;
        _aelfClientOptions = aelfClientOptions.Value;
        _aelfAccountOptions = aelfAccountOptions.Value;

        var clientBuilder = new AElfClientBuilder();
        foreach (var (alias, clientConfig) in aelfClientOptions.Value.ClientConfigs)
        {
            var client = clientBuilder
                .UseEndpoint(clientConfig.Endpoint)
                .ManagePeerInfo(clientConfig.UserName, clientConfig.Password)
                .SetHttpTimeout(clientConfig.Timeout)
                .Build();
            _aelfClientProvider.SetClient(client, alias: alias);
        }
    }

    public async Task<byte[]> ViewAsync(string contractAddress, string methodName, IMessage parameter, string clientAlias,
        string accountAlias = "Default")
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: accountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseContract(contractAddress)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        return await PerformViewAsync(aelfClient, tx);
    }
    
    public async Task<byte[]> ViewSystemAsync(string systemContractName, string methodName, IMessage parameter, string clientAlias,
        string accountAlias = "Default")
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var privateKey = _aelfAccountProvider.GetPrivateKey(alias: accountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(privateKey)
            .UseSystemContract(systemContractName)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        return await PerformViewAsync(aelfClient, tx);
    }

    private async Task<byte[]> PerformViewAsync(AElfClient aelfClient, Transaction tx)
    {
        var result = await aelfClient.ExecuteTransactionAsync(new ExecuteTransactionDto
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });
        return ByteArrayHelper.HexStringToByteArray(result);
    }
}