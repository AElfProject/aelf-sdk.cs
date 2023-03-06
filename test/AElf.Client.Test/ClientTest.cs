using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Types;
using AElf.Client.Dto;
using AElf.Client.Extensions;
using AElf.Client.Service;
using AElf.Contracts.MultiToken;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp.Threading;
using Xunit;
using Xunit.Abstractions;
using Address = AElf.Types.Address;
using Hash = AElf.Types.Hash;

namespace AElf.Client.Test;

public class ClientTest
{
    private const string BaseUrl = "http://127.0.0.1:8000";

    private string _genesisAddress;
    private string GenesisAddress => GetGenesisContractAddress();
        
    // example contract-method-name
    private string ContractMethodName => "GetContractAddressByName";

    // Address and privateKey of a node.
    private readonly string _address;
    private const string PrivateKey = "cd86ab6347d8e52bbbe8532141fc59ce596268143a308d1d40fedf385528b458";

    private AElfClient Client { get; }
    private readonly ITestOutputHelper _testOutputHelper;

    private const string UserName = "test1";
    private const string Password = "test";
    private const string Version = "1.2.3.0";

    public ClientTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Client = new AElfClient(BaseUrl, userName: UserName, password: Password);
            
        // To get address from privateKey.s
        _address = Client.GetAddressFromPrivateKey(PrivateKey);
    }

    #region block

    [Fact]
    public async Task GetBlockHeightTest()
    {
        var height = await Client.GetBlockHeightAsync();
        height.ShouldBeGreaterThanOrEqualTo(1);
        _testOutputHelper.WriteLine(height.ToString());
    }

    [Fact]
    public async Task GetBlock_Test()
    {
        var height = await Client.GetBlockHeightAsync();
        var blockByHeight = await Client.GetBlockByHeightAsync(height);
        var blockByHash = await Client.GetBlockByHashAsync(blockByHeight.BlockHash);
            
        var blockByHeightSerialized =JsonConvert.SerializeObject(blockByHeight, Formatting.Indented);
        var blockByHashSerialized =JsonConvert.SerializeObject(blockByHash, Formatting.Indented);
            
        blockByHeightSerialized.ShouldBe(blockByHashSerialized);
        blockByHeight.Header.Height.ShouldBe(height);
        blockByHeight.IsComplete().ShouldBeTrue();
            
        var block = JsonConvert.SerializeObject(blockByHeight, Formatting.Indented);
        _testOutputHelper.WriteLine(block);
    }

    [Fact]
    public async Task GetBlockByHeight_Failed_Test()
    {
        const int timeOut = 60;
        var httpService = new HttpService(timeOut);
        const int heightNotExist = int.MaxValue;
        var errorResponse = await httpService.GetResponseAsync<WebAppErrorResponse>(
            $"{BaseUrl}/api/blockChain/blockByHeight?blockHeight={heightNotExist}&includeTransactions=false",
            expectedStatusCode: HttpStatusCode.Forbidden);
        errorResponse.Error.Code.ShouldBe(Error.NotFound.ToString());
        errorResponse.Error.Message.ShouldBe(Error.Message[Error.NotFound]);
        var str = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
        _testOutputHelper.WriteLine(str);
    }

    #endregion

    #region chain

    [Fact]
    public async Task GetChainStatus_Test()
    {
        var chainStatusDto = await Client.GetChainStatusAsync();
            
        chainStatusDto.Branches.Count.ShouldBeGreaterThanOrEqualTo(1);
        chainStatusDto.Branches.First().Key.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.Branches.First().Value.ShouldBeGreaterThanOrEqualTo(1);
        chainStatusDto.ChainId.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.BestChainHash.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.BestChainHeight.ShouldBeGreaterThanOrEqualTo(1);
        chainStatusDto.GenesisBlockHash.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.GenesisContractAddress.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.LongestChainHash.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.LongestChainHeight.ShouldBeGreaterThanOrEqualTo(1);
        chainStatusDto.LastIrreversibleBlockHash.ShouldNotBeNullOrWhiteSpace();
        chainStatusDto.LastIrreversibleBlockHeight.ShouldBeGreaterThanOrEqualTo(1);
            
        var chainStatus = JsonConvert.SerializeObject(chainStatusDto, Formatting.Indented);
        _testOutputHelper.WriteLine(chainStatus);
    }

    #endregion

    #region net

    /// <summary>
    /// Work in multiple nodes.(>=2)
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "Redo this later.")]
    public async Task AddPeer_Test()
    {
        // add ipAddress
        var addressToAdd = "192.168.199.122:7003";

        var addSuccess = await Client.AddPeerAsync(addressToAdd, UserName, Password);
        addSuccess.ShouldBeTrue();
        _testOutputHelper.WriteLine($"Added ipAddress: {addressToAdd}");
    }

    [Fact(Skip = "Redo this later.")]
    public async Task RemovePeer_Test()
    {
        var peers = await Client.GetPeersAsync(false);
        peers.ShouldNotBeEmpty();

        var peerToRemoveAddress = peers[0].IpAddress;
        var removeSuccess = await Client.RemovePeerAsync(peerToRemoveAddress, UserName, Password);
        Assert.True(removeSuccess);
        _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");
    }

    [Fact]
    public async Task GetPeers_Test()
    {
        var peers = await Client.GetPeersAsync(false);
        Assert.True(peers != null);
        var peersInfo = JsonConvert.SerializeObject(peers, Formatting.Indented);
        _testOutputHelper.WriteLine(peersInfo);
    }

    [Fact]
    public async Task GetNetworkInfo_Test()
    {
        var netWorkInfo = await Client.GetNetworkInfoAsync();
        Assert.True(netWorkInfo != null);
        netWorkInfo.Version.ShouldBe(Version);
        var info = JsonConvert.SerializeObject(netWorkInfo, Formatting.Indented);
        _testOutputHelper.WriteLine(info);
    }

    #endregion

    #region transaction

    [Fact]
    public async Task GetTaskQueueStatus_Test()
    {
        var taskQueueStatus = await Client.GetTaskQueueStatusAsync();
        taskQueueStatus.ShouldNotBeEmpty();

        var queueStatus = JsonConvert.SerializeObject(taskQueueStatus, Formatting.Indented);
        _testOutputHelper.WriteLine(queueStatus);
    }

    [Fact]
    public async Task GetTransactionPoolStatus_Test()
    {
        var poolStatus = await Client.GetTransactionPoolStatusAsync();
        Assert.True(poolStatus != null);

        var status = JsonConvert.SerializeObject(poolStatus, Formatting.Indented);
        _testOutputHelper.WriteLine(status);
    }

    [Fact]
    public async Task ExecuteTransaction_Test()
    {
        var toAddress = await Client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
        var methodName = "GetNativeTokenInfo";
        var param = new Empty();

        var transaction = await Client.GenerateTransactionAsync(_address, toAddress.ToBase58(), methodName, param);
        var txWithSign = Client.SignTransaction(PrivateKey, transaction);

        var transactionResult = await Client.ExecuteTransactionAsync(new ExecuteTransactionDto
        {
            RawTransaction = txWithSign.ToByteArray().ToHex()
        });
        Assert.True(transactionResult != null);

        var tokenInfo = TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(transactionResult));
        Assert.True(tokenInfo.Symbol == "ELF");
    }

    [Fact]
    public async Task CreateRawTransaction_Test()
    {
        var address = GenesisAddress;
        var status = await Client.GetChainStatusAsync();
        var height = status.BestChainHeight;
        var blockHash = status.BestChainHash;

        var createRaw = await Client.CreateRawTransactionAsync(new CreateRawTransactionInput
        {
            From = _address,
            To = address,
            MethodName = ContractMethodName,
            Params = new JObject
            {
                ["value"] = HashHelper.ComputeFrom("AElf.ContractNames.Token").Value.ToBase64()
            }.ToString(),
            RefBlockNumber = height,
            RefBlockHash = blockHash
        });

        createRaw.RawTransaction.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ExecuteRawTransaction_Test()
    {
        var address = GenesisAddress;
        var status = await Client.GetChainStatusAsync();
        var height = status.BestChainHeight;
        var blockHash = status.BestChainHash;

        var createRaw = await Client.CreateRawTransactionAsync(new CreateRawTransactionInput
        {
            From = _address,
            To = address,
            MethodName = ContractMethodName,
            Params = new JObject
            {
                ["value"] = HashHelper.ComputeFrom("AElf.ContractNames.Consensus").Value.ToBase64()
            }.ToString(),
            RefBlockNumber = height,
            RefBlockHash = blockHash
        });

        var transactionId = HashHelper.ComputeFrom(ByteArrayHelper.HexStringToByteArray(createRaw.RawTransaction));
        var signature = GetSignatureWith(PrivateKey, transactionId.ToByteArray()).ToHex();
        var rawTransactionResult = await Client.ExecuteRawTransactionAsync(new ExecuteRawTransactionDto
        {
            RawTransaction = createRaw.RawTransaction,
            Signature = signature
        });

        rawTransactionResult.ShouldNotBeEmpty();
        var consensusAddress =
            (await Client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Consensus")))
            .ToBase58();

        var addressResult = rawTransactionResult.Trim('"');
        _testOutputHelper.WriteLine(rawTransactionResult);
        addressResult.ShouldBe(consensusAddress);
    }

    [Fact]
    public async Task SendRawTransaction_Test()
    {
        var toAddress = GenesisAddress;
        var status = await Client.GetChainStatusAsync();
        var height = status.BestChainHeight;
        var blockHash = status.BestChainHash;

        var createRaw = await Client.CreateRawTransactionAsync(new CreateRawTransactionInput
        {
            From = _address,
            To = toAddress,
            MethodName = ContractMethodName,
            Params = new JObject
            {
                ["value"] = HashHelper.ComputeFrom("AElf.ContractNames.Token").Value.ToBase64()
            }.ToString(),
            RefBlockNumber = height,
            RefBlockHash = blockHash
        });
        createRaw.RawTransaction.ShouldNotBeEmpty();

        var transactionId = HashHelper.ComputeFrom(ByteArrayHelper.HexStringToByteArray(createRaw.RawTransaction));
        var signature = GetSignatureWith(PrivateKey, transactionId.ToByteArray()).ToHex();

        var rawTransactionResult = await Client.SendRawTransactionAsync(new SendRawTransactionInput
        {
            Transaction = createRaw.RawTransaction,
            Signature = signature,
            ReturnTransaction = true
        });

        rawTransactionResult.ShouldNotBeNull();
        rawTransactionResult.Transaction.ShouldNotBeNull();
        rawTransactionResult.TransactionId.ShouldNotBeNullOrWhiteSpace();

        var result = JsonConvert.SerializeObject(rawTransactionResult, Formatting.Indented);
        _testOutputHelper.WriteLine(result);
    }

    [Fact]
    public async Task SendTransaction_Test()
    {
        var toAddress = GenesisAddress;
        var methodName = ContractMethodName;
        var param = HashHelper.ComputeFrom("AElf.ContractNames.Vote");

        var transaction = await Client.GenerateTransactionAsync(_address, toAddress, methodName, param);
        var txWithSign = Client.SignTransaction(PrivateKey, transaction);

        var result = await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = txWithSign.ToByteArray().ToHex()
        });

        result.ShouldNotBeNull();
        result.TransactionId.ShouldNotBeNull();
        _testOutputHelper.WriteLine(result.TransactionId);
    }

    [Fact]
    public async Task SendTransactions_Test()
    {
        var toAddress = GenesisAddress;
        var methodName = ContractMethodName;
        var param1 = HashHelper.ComputeFrom("AElf.ContractNames.Token");
        var param2 = HashHelper.ComputeFrom("AElf.ContractNames.Vote");

        var parameters = new List<Hash> {param1, param2};
        var sb = new StringBuilder();

        foreach (var param in parameters)
        {
            var tx = await Client.GenerateTransactionAsync(_address, toAddress, methodName, param);
            var txWithSign = Client.SignTransaction(PrivateKey, tx);
            sb.Append(txWithSign.ToByteArray().ToHex() + ',');
        }

        var result1 = await Client.SendTransactionsAsync(new SendTransactionsInput
        {
            RawTransactions = sb.ToString().Substring(0, sb.Length - 1)
        });

        Assert.True(result1 != null);
        result1.Length.ShouldBe(2);
        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(result1));

    }

    [Fact]
    public async Task GetTransactionResult_Test()
    {
        var firstBlockDto = await Client.GetBlockByHeightAsync(1, true);
        var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();

        var transactionResultDto = await Client.GetTransactionResultAsync(transactionId);
        Assert.True(transactionResultDto.Status == TransactionResultStatus.Mined.ToString().ToUpper());
    }

    [Fact]
    public async Task GetTransactionResults_Test()
    {
        var firstBlockDto = await Client.GetBlockByHeightAsync(1, true);
        var blockHash = firstBlockDto.BlockHash;

        var transactionResults = await Client.GetTransactionResultsAsync(blockHash, 0, 2);
        foreach (var transaction in transactionResults)
        {
            Assert.True(transaction.Status == TransactionResultStatus.Mined.ToString().ToUpper());
        }
    }

    [Fact]
    public async Task GetMerklePathByTransactionId_Test()
    {
        var firstBlockDto = await Client.GetBlockByHeightAsync(1, true);
        var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();
        var merklePathDto = await Client.GetMerklePathByTransactionIdAsync(transactionId);
        Assert.True(merklePathDto != null);

        var merklePath = JsonConvert.SerializeObject(merklePathDto, Formatting.Indented);
        merklePath.ShouldNotBeEmpty();

        _testOutputHelper.WriteLine(merklePath);
    }

    [Fact]
    public async Task GetChainId_Test()
    {
        var chainId = await Client.GetChainIdAsync();
        chainId.ShouldBe(9992731);
    }

    [Fact]
    public async Task IsConnected_Test()
    {
        var isConnected = await Client.IsConnectedAsync();
        isConnected.ShouldBeTrue();
    }

    [Fact]
    public async Task GetGenesisContractAddress_Test()
    {
        var genesisAddress = await Client.GetGenesisContractAddressAsync();
        genesisAddress.ShouldNotBeEmpty();

        var address = await Client.GetContractAddressByNameAsync(Hash.Empty);
        var genesisAddress2 = address.ToBase58();
        Assert.True(genesisAddress == genesisAddress2);
    }

    [Fact]
    public async Task GetFormattedAddress_Test()
    {
        var result = await Client.GetFormattedAddressAsync(Address.FromBase58(_address));
        _testOutputHelper.WriteLine(result);
        Assert.True(result == $"ELF_{_address}_AELF");
    }

    [Fact]
    public void GetNewKeyPairInfo_Test()
    {
        var output = Client.GenerateKeyPairInfo();
        var addressOutput = JsonConvert.SerializeObject(output);
        _testOutputHelper.WriteLine(addressOutput);
    }

    [Fact]
    public async Task Transfer_Test()
    {
        var toAccount = Client.GenerateKeyPairInfo().Address;
        var toAddress = await Client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
        var methodName = "Transfer";
        var param = new TransferInput
        {
            To = new Address {Value = Address.FromBase58(toAccount).Value},
            Symbol = "ELF",
            Amount = 1000
        };

        var transaction = await Client.GenerateTransactionAsync(_address, toAddress.ToBase58(), methodName, param);
        var txWithSign = Client.SignTransaction(PrivateKey, transaction);

        var result = await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = txWithSign.ToByteArray().ToHex()
        });

        result.ShouldNotBeNull();
        _testOutputHelper.WriteLine(result.TransactionId);

        await Task.Delay(4000);
        var transactionResult = await Client.GetTransactionResultAsync(result.TransactionId);
        transactionResult.Status.ShouldBe(TransactionResultStatus.Mined.ToString().ToUpper());
        var transactionFees = transactionResult.GetTransactionFees();
        transactionFees.First().Key.ShouldBe("ELF");
        transactionFees.First().Value.ShouldBeGreaterThan(0L);
        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(transactionFees, Formatting.Indented));

        var paramGetBalance = new GetBalanceInput
        {
            Symbol = "ELF",
            Owner = new Address {Value = Address.FromBase58(toAccount).Value}
        };

        var transactionGetBalance =
            await Client.GenerateTransactionAsync(_address, toAddress.ToBase58(), "GetBalance", paramGetBalance);
        var txWithSignGetBalance = Client.SignTransaction(PrivateKey, transactionGetBalance);

        var transactionGetBalanceResult = await Client.ExecuteTransactionAsync(new ExecuteTransactionDto
        {
            RawTransaction = txWithSignGetBalance.ToByteArray().ToHex()
        });
        Assert.True(transactionResult != null);

        var balance =
            GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(transactionGetBalanceResult));
        balance.Balance.ShouldBe(1000L);
    }

    [Fact]
    public void GetTransactionFee_Test()
    {
        var transactionResultDto = new TransactionResultDto
        {
            Logs = new[]
            {
                new LogEventDto
                {
                    Name = "TransactionFeeCharged",
                    NonIndexed = Convert.ToBase64String((new TransactionFeeCharged {Symbol = "ELF", Amount = 1000}).ToByteArray())
                },
                new LogEventDto
                {
                    Name = "ResourceTokenCharged",
                    NonIndexed = Convert.ToBase64String((new ResourceTokenCharged {Symbol = "READ", Amount = 800}).ToByteArray())
                },
                new LogEventDto
                {
                    Name = "ResourceTokenCharged",
                    NonIndexed = Convert.ToBase64String((new ResourceTokenCharged {Symbol = "WRITE", Amount = 600}).ToByteArray())
                },
                new LogEventDto
                {
                    Name = "ResourceTokenOwned",
                    NonIndexed = Convert.ToBase64String((new ResourceTokenOwned {Symbol = "READ", Amount = 200}).ToByteArray())
                }
            }
        };

        var transactionFees = transactionResultDto.GetTransactionFees();
        transactionFees.Count.ShouldBe(3);
        transactionFees["ELF"].ShouldBe(1000);
        transactionFees["READ"].ShouldBe(800);
        transactionFees["WRITE"].ShouldBe(600);


        transactionResultDto = new TransactionResultDto();
        transactionFees = transactionResultDto.GetTransactionFees();
        transactionFees.Count.ShouldBe(0);
    }

    [Fact]
    public async void CalculateTransactionFee_Test()
    {
        var address = GenesisAddress;
        var status = await Client.GetChainStatusAsync();
        var height = status.BestChainHeight;
        var blockHash = status.BestChainHash;

        var createRaw = await Client.CreateRawTransactionAsync(new CreateRawTransactionInput
        {
            From = _address,
            To = address,
            MethodName = ContractMethodName,
            Params = new JObject
            {
                ["value"] = HashHelper.ComputeFrom("AElf.ContractNames.Token").Value.ToBase64()
            }.ToString(),
            RefBlockNumber = height,
            RefBlockHash = blockHash
        });

        createRaw.RawTransaction.ShouldNotBeEmpty();
        var input = new CalculateTransactionFeeInput
        {
            RawTransaction = createRaw.RawTransaction
        };
        var transactionFeeOutput = await Client.CalculateTransactionFeeAsync(input);
        transactionFeeOutput.Success.ShouldBe(true);
        transactionFeeOutput.TransactionFee["ELF"].ShouldBeGreaterThan(18000000);
        transactionFeeOutput.TransactionFee["ELF"].ShouldBeLessThanOrEqualTo(19000000);




    }

    #endregion

    #region private methods

    private string GetGenesisContractAddress()
    {
        if (_genesisAddress != null) return _genesisAddress;

        var statusDto = AsyncHelper.RunSync(Client.GetChainStatusAsync);
        _genesisAddress = statusDto.GenesisContractAddress;

        return _genesisAddress;
    }

    private ByteString GetSignatureWith(string privateKey, byte[] txData)
    {
        // Sign the hash
        var signature = CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(privateKey), txData);
        return ByteString.CopyFrom(signature);
    }

    #endregion
}