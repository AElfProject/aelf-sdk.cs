using System;
using System.Text;
using System.Threading.Tasks;
using AElf.Types;
using AElf.Client.Dto;
using AElf.Inscription;
using Google.Protobuf;
using Xunit;
using Xunit.Abstractions;

namespace AElf.Client.Test;

public class InscriptionTest
{
    private const string BaseUrl = "https://aelf-test-node.aelf.io";

    // example contract-method-name
    private string ContractMethodName => "GetContractAddressByName";

    // Address and privateKey of a node.
    private readonly string _address;
    private const string PrivateKey = "345af4fbb6d6142f15ec0f5ebae680195bb655f88f22573ac01222bbf537e9ca";

    private AElfClient Client { get; }
    private readonly ITestOutputHelper _testOutputHelper;

    private const string UserName = "test1";
    private const string Password = "test";

    public InscriptionTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Client = new AElfClient(BaseUrl, userName: UserName, password: Password);

        // To get address from privateKey.s
        _address = Client.GetAddressFromPrivateKey(PrivateKey);
    }

    [Fact]
    public async Task Test_Inscribe()
    {
        var tokenContractAddress =
            await Client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));

        // Using example https://ethscriptions.com/ethscriptions/0x6317e5bcc0b2d41f8de41d30b77e8e49e465766ca634f024c8ae913898620e94

        var dataUrl =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAYAAABccqhmAAAACXBIWXMAAAsSAAALEgHS3X78AAAFUmlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSfvu78nIGlkPSdXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQnPz4KPHg6eG1wbWV0YSB4bWxuczp4PSdhZG9iZTpuczptZXRhLycgeDp4bXB0az0nSW1hZ2U6OkV4aWZUb29sIDEyLjY0Jz4KPHJkZjpSREYgeG1sbnM6cmRmPSdodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjJz4KCiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0nJwogIHhtbG5zOmRjPSdodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyc+CiAgPGRjOmRlc2NyaXB0aW9uPgogICA8cmRmOkFsdD4KICAgIDxyZGY6bGkgeG1sOmxhbmc9J3gtZGVmYXVsdCc+eyYjMzk7bmFtZSYjMzk7OiYjMzk7QmV2JiMzOTssJiMzOTtudW1iZXImIzM5OzozMDUwLCYjMzk7ZGVzY3JpcHRpb24mIzM5OzomIzM5O0h5cHBvY3JpdGV6IGlzIG9uZSBvZiB0aGUgZmlyc3Qgb3JpZ2luYWwgY29sbGVjdGlvbnMgbWFkZSBzcGVjaWZpY2FsbHkgZm9yIEV0aHNjcmlwdGlvbnMuIEEgY29sbGVjdGlvbiBvZiBkZWdlbiBoeXBwb3MgZm9yIFdlYjMgcGVvcGxlIHdobyBoYXRlIHRvIGxvdmUgV2ViMy4mIzM5OywmIzM5O2V4dGVybmFsX3VybCYjMzk7OiYjMzk7aHR0cHM6Ly93d3cuaHlwcG9jcml0ZXouY29tJiMzOTssJiMzOTthdHRyaWJ1dGVzJiMzOTs6W3smIzM5O3RyYWl0X3R5cGUmIzM5OzomIzM5O1RlYW0mIzM5OywmIzM5O3ZhbHVlJiMzOTs6JiMzOTtCbHVlJiMzOTt9LHsmIzM5O3RyYWl0X3R5cGUmIzM5OzomIzM5O0hhbmQmIzM5OywmIzM5O3ZhbHVlJiMzOTs6JiMzOTtIb21lLVJ1bm5lciYjMzk7fSx7JiMzOTt0cmFpdF90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";

        var methodName = "Transfer";
        var param = new TransferInputWithInscription
        {
            To = new Address { Value = Address.FromBase58("hvydyxP4xbM85oe2ipneoMFFtxzF9xi2u6h2MxZxLPTxbnRQL").Value },
            Symbol = "ELF",
            Amount = 1,
            Memo = "",
            Inscription = ByteString.CopyFrom(Encoding.UTF8.GetBytes(dataUrl))
        };
        var ownerAddress = Client.GetAddressFromPrivateKey(PrivateKey);
        var transaction =
            await Client.GenerateTransactionAsync(ownerAddress, tokenContractAddress.ToBase58(), methodName, param);
        var txWithSign = Client.SignTransaction(PrivateKey, transaction);

        var result = await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = txWithSign.ToByteArray().ToHex()
        });

        await Task.Delay(4000);
        // After the transaction is mined, query the execution results.
        var transactionResult = await Client.GetTransactionResultAsync(result.TransactionId);
        _testOutputHelper.WriteLine($"TransactionId is: {result.TransactionId}");
        _testOutputHelper.WriteLine(transactionResult.Status);
    }
}