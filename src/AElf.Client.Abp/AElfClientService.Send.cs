using AElf.Client.Dto;
using Google.Protobuf;

namespace AElf.Client.Abp;

public partial class AElfClientService
{
    public async Task<Transaction> SendAsync(string contractAddress, string methodName, IMessage parameter,
        string clientAlias)
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: _clientConfigOptions.AccountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseContract(contractAddress)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        await PerformSendAsync(aelfClient, tx);
        return tx;
    }

    public async Task<Transaction> SendSystemAsync(string systemContractName, string methodName, IMessage parameter,
        string clientAlias)
    {
        var aelfClient = _aelfClientProvider.GetClient(alias: clientAlias);
        var aelfAccount = _aelfAccountProvider.GetPrivateKey(alias: _clientConfigOptions.AccountAlias);
        var tx = new TransactionBuilder(aelfClient)
            .UsePrivateKey(aelfAccount)
            .UseSystemContract(systemContractName)
            .UseMethod(methodName)
            .UseParameter(parameter)
            .Build();
        await PerformSendAsync(aelfClient, tx);
        return tx;
    }

    private static async Task PerformSendAsync(AElfClient aelfClient, Transaction tx)
    {
        var result = await aelfClient.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });
    }
}