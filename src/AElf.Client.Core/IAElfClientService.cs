using AElf.Client.Dto;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace AElf.Client.Core;

public interface IAElfClientService
{
    /// <summary>
    /// Build a transaction to view contract state and return the result.
    /// </summary>
    /// <param name="contractAddress">Transaction.To</param>
    /// <param name="methodName">Transaction.MethodName</param>
    /// <param name="parameter">Transaction.Parameter</param>
    /// <param name="clientAlias">Which client to use</param>
    /// <param name="accountAlias">Which account to use</param>
    /// <typeparam name="T">Only IMessage type</typeparam>
    /// <returns>Contract call result</returns>
    Task<T> ViewAsync<T>(string contractAddress, string methodName, IMessage parameter, string clientAlias,
        string accountAlias = "Default") where T : IMessage, new();

    /// <summary>
    /// Build a transaction to view system contract state and return the result.
    /// </summary>
    /// <param name="systemContractName">The System Contract Name with the format <code>AElf.ContractNames.*</code></param>
    /// <param name="methodName">Transaction.MethodName</param>
    /// <param name="parameter">Transaction.Parameter</param>
    /// <param name="clientAlias">Which client to use</param>
    /// <param name="accountAlias">Which account to use</param>
    /// <typeparam name="T">Only IMessage type</typeparam>
    /// <returns>Contract call result</returns>
    /// <exception cref="ArgumentException">Throw <code>Invalid system contract name</code> if <code>systemContractName</code>
    /// is not starts with <code>AElf.ContractNames.</code></exception>
    Task<T> ViewSystemAsync<T>(string systemContractName, string methodName, IMessage parameter,
        string clientAlias, string accountAlias = "Default") where T : IMessage, new();

    /// <summary>
    /// Send a transaction to a Contract.
    /// </summary>
    /// <param name="contractAddress">Transaction.tO</param>
    /// <param name="methodName">Transaction.MethodName</param>
    /// <param name="parameter">Transaction.Parameter</param>
    /// <param name="clientAlias">Which client to use</param>
    /// <param name="accountAlias">Which account to use</param>
    /// <returns>Contract call result</returns>
    Task<Transaction> SendAsync(string contractAddress, string methodName, IMessage parameter,
        string clientAlias, string? accountAlias = null);

    /// <summary>
    /// Send a transaction to a System Contract.
    /// </summary>
    /// <param name="systemContractName">The System Contract Name with the format <code>AElf.ContractNames.*</code></param>
    /// <param name="methodName">Transaction.MethodName</param>
    /// <param name="parameter">Transaction.Parameter</param>
    /// <param name="clientAlias">Which client to use</param>
    /// <param name="accountAlias">Which account to use</param>
    /// <returns>Contract call result</returns>
    /// <exception cref="ArgumentException">Throw <code>Invalid system contract name</code> if <code>systemContractName</code>
    /// is not starts with <code>AElf.ContractNames.</code></exception>
    Task<Transaction> SendSystemAsync(string systemContractName, string methodName, IMessage parameter,
        string clientAlias, string? accountAlias = null);

    Task<TransactionResult> GetTransactionResultAsync(string transactionId, string clientAlias);

    Task<ChainStatusDto> GetChainStatusAsync(string clientAlias);

    Task<Address> GetGenesisContractAddressAsync(string clientAlias);

    Task<MerklePath> GetMerklePathByTransactionIdAsync(string transactionId, string clientAlias);

    Task<FileDescriptorSet> GetContractFileDescriptorSetAsync(string contractAddress, string clientAlias);
}