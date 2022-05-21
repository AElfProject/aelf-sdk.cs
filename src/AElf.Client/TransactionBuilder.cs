using AElf.Cryptography;
using Google.Protobuf;

namespace AElf.Client;

public class TransactionBuilder
{
    private readonly AElfClient _aelfClient;

    private string PrivateKey { get; set; }
    private string ContractAddress { get; set; }
    private string MethodName { get; set; }
    private IMessage Parameter { get; set; }

    public TransactionBuilder(AElfClient aelfClient)
    {
        _aelfClient = aelfClient;
        PrivateKey = AElfClientConstants.DefaultPrivateKey;
    }

    public TransactionBuilder UsePrivateKey(string privateKey)
    {
        PrivateKey = privateKey;
        return this;
    }

    public TransactionBuilder UseSystemContract(string systemContractName)
    {
        ContractAddress = _aelfClient.GetContractAddressByNameAsync(HashHelper.ComputeFrom(systemContractName))
            .Result.ToBase58();
        return this;
    }

    public TransactionBuilder UseContract(string contractAddress)
    {
        ContractAddress = contractAddress;
        return this;
    }

    public TransactionBuilder UseMethod(string methodName)
    {
        MethodName = methodName;
        return this;
    }

    public TransactionBuilder UseParameter(IMessage parameter)
    {
        Parameter = parameter;
        return this;
    }

    public Transaction Build()
    {
        var keyPair = CryptoHelper.FromPrivateKey(ByteArrayHelper.HexStringToByteArray(PrivateKey));
        var from = Address.FromPublicKey(keyPair.PublicKey).ToBase58();
        var unsignedTx = _aelfClient.GenerateTransactionAsync(from, ContractAddress, MethodName, Parameter).Result;
        var signedTx = _aelfClient.SignTransaction(PrivateKey, unsignedTx);
        return signedTx;
    }
}