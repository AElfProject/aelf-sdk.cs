using AElf.Client.Dto;
using Google.Protobuf;

namespace AElf.Client.Abp;

public partial class AElfClientService
{
    public async Task<string> SendAsync(string contractAddress, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default")
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: accountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseContract(contractAddress)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        return await PerformSendAsync(aelfClient, tx);
    }

    public async Task<string> SendSystemAsync(string systemContractName, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default")
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: accountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseSystemContract(systemContractName)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        return await PerformSendAsync(aelfClient, tx);
    }

    private static async Task<string> PerformSendAsync(AElfClient aelfClient, Transaction tx)
    {
        var result = await aelfClient.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });
        return result.TransactionId;
    }
}