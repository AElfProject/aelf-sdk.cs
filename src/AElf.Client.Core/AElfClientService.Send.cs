using AElf.Client.Dto;
using AElf.Client.Services;
using Google.Protobuf;

namespace AElf.Client.Core;

public partial class AElfClientService
{
    public async Task<Transaction> SendAsync(string contractAddress, string methodName, IMessage parameter,
        string clientAlias, string? accountAlias = null)
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
        string clientAlias, string? accountAlias = null)
    {
        if (!systemContractName.StartsWith("AElf.ContractNames."))
        {
            throw new ArgumentException("Invalid system contract name.");
        }

        if (systemContractName == AElfClientCoreConstants.GenesisSmartContractName)
        {
            return await SendAsync((await GetGenesisContractAddressAsync(clientAlias)).ToBase58(), methodName,
                parameter, clientAlias, accountAlias);
        }

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

    private static async Task PerformSendAsync(ITransactionAppService aelfClient, Transaction tx)
    {
        await aelfClient.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });
    }
}