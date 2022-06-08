using Google.Protobuf;

namespace AElf.Client.Abp;

public interface IAElfClientService
{
    Task<byte[]> ViewAsync(string contractAddress, string methodName, IMessage parameter, string clientAlias,
        string accountAlias = "Default");

    Task<byte[]> ViewSystemAsync(string systemContractName, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default");

    Task<string> SendAsync(string contractAddress, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default");

    Task<string> SendSystemAsync(string systemContractName, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default");

    Task<TransactionResult> GetTransactionResultAsync(string transactionId, string clientAlias);
}