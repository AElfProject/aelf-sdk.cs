using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Types;
using AElf.Client.Dto;
using AElf.Client.Extension;
using AElf.Client.MultiToken;
using AElf.Client.Service;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp.Threading;
using Xunit;
using Xunit.Abstractions;
using Address = AElf.Types.Address;
using Hash = AElf.Types.Hash;

namespace AElf.Client.Test
{
    public class ClientTest
    {
        private const string BaseUrl = "Http://127.0.0.1:8001";

        private string _genesisAddress;
        private string GenesisAddress => GetGenesisContractAddress();
        
        // example contract-method-name
        private string ContractMethodName => "GetContractAddressByName";

        // Address and privateKey of a node.
        private readonly string _address;
        private const string PrivateKey = "85447a83ce2ed09a4a2424304e54f9e3dbf5f2c1d28a4554447f868a6ff3565a";

        private AElfClient Client { get; }
        private readonly ITestOutputHelper _testOutputHelper;

        public ClientTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Client = new AElfClient(BaseUrl);
            
            // To get address from privateKey.s
            _address = Client.GetAddressFromPrivateKey(PrivateKey);
        }

        #region block

        [Fact]
        public async Task GetBlockHeightTest()
        {
            var height = await Client.GetBlockHeightAsync();
            height.ShouldNotBeNull();
            _testOutputHelper.WriteLine(height.ToString());
        }

        [Fact]
        public async Task GetBlock_ByHeightAsyncTest()
        {
            var height = await Client.GetBlockHeightAsync();
            var blockDto = await Client.GetBlockByHeightAsync(height);
            Assert.True(blockDto != null);

            var block = JsonConvert.SerializeObject(blockDto, Formatting.Indented);
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

        [Fact]
        public async Task GetBlockAsync_Success_Test()
        {
            var chainStatusDto = await Client.GetChainStatusAsync();
            var genesisHash = chainStatusDto.GenesisBlockHash;

            var blockDto = await Client.GetBlockByHashAsync(genesisHash, true);
            Assert.True(blockDto != null);

            var block = JsonConvert.SerializeObject(blockDto, Formatting.Indented);
            _testOutputHelper.WriteLine(block);
        }

        #endregion

        #region chain

        [Fact]
        public async Task GetChainStatusAsync_Test()
        {
            var chainStatusDto = await Client.GetChainStatusAsync();
            Assert.True(chainStatusDto != null);

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
        public async Task AddPeerAsync_Test()
        {
            // add ipAddress
            var addressToAdd = "192.168.199.122:7003";

            var addSuccess = await Client.AddPeerAsync(addressToAdd);
            addSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Added ipAddress: {addressToAdd}");
        }

        [Fact(Skip = "Redo this later.")]
        public async Task RemovePeerAsync_Test()
        {
            var peers = await Client.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();

            var peerToRemoveAddress = peers[0].IpAddress;
            var removeSuccess = await Client.RemovePeerAsync(peerToRemoveAddress);
            Assert.True(removeSuccess);
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");
        }

        [Fact]
        public async Task GetPeersAsync_Test()
        {
            var peers = await Client.GetPeersAsync(false);
            Assert.True(peers != null);
            var peersInfo = JsonConvert.SerializeObject(peers, Formatting.Indented);
            _testOutputHelper.WriteLine(peersInfo);
        }

        [Fact]
        public async Task GetNetworkInfoAsync_Test()
        {
            var netWorkInfo = await Client.GetNetworkInfoAsync();
            Assert.True(netWorkInfo != null);
            var info = JsonConvert.SerializeObject(netWorkInfo, Formatting.Indented);
            _testOutputHelper.WriteLine(info);
        }

        #endregion

        #region transaction

        [Fact]
        public async Task GetTaskQueueStatusAsync_Test()
        {
            var taskQueueStatus = await Client.GetTaskQueueStatusAsync();
            taskQueueStatus.ShouldNotBeEmpty();

            var queueStatus = JsonConvert.SerializeObject(taskQueueStatus, Formatting.Indented);
            _testOutputHelper.WriteLine(queueStatus);
        }

        [Fact]
        public async Task GetTransactionPoolStatusAsync_Test()
        {
            var poolStatus = await Client.GetTransactionPoolStatusAsync();
            Assert.True(poolStatus != null);

            var status = JsonConvert.SerializeObject(poolStatus, Formatting.Indented);
            _testOutputHelper.WriteLine(status);
        }

        [Fact]
        public async Task ExecuteTransactionAsync_Test()
        {
            var toAddress = GenesisAddress;
            var methodName = ContractMethodName;
            var param = Hash.FromString("AElf.ContractNames.TokenConverter");

            var transaction = await Client.GenerateTransaction(_address, toAddress, methodName, param);
            var txWithSign = Client.SignTransaction(PrivateKey, transaction);

            var transactionResult = await Client.ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });
            Assert.True(transactionResult != null);

            var addressResult = Address.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(transactionResult));
            var address = await Client.GetContractAddressByName(param);
            Assert.True(address == addressResult);
        }

        [Fact]
        public async Task CreateRawTransactionAsync_Test()
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
                    ["value"] = Hash.FromString("AElf.ContractNames.Token").Value.ToBase64()
                }.ToString(),
                RefBlockNumber = height,
                RefBlockHash = blockHash
            });

            createRaw.RawTransaction.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ExecuteRawTransactionAsync_Test()
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
                    ["value"] = Hash.FromString("AElf.ContractNames.Consensus").Value.ToBase64()
                }.ToString(),
                RefBlockNumber = height,
                RefBlockHash = blockHash
            });

            var transactionId = Hash.FromRawBytes(ByteArrayHelper.HexStringToByteArray(createRaw.RawTransaction));
            var signature = GetSignatureWith(PrivateKey, transactionId.ToByteArray()).ToHex();
            var rawTransactionResult = await Client.ExecuteRawTransactionAsync(new ExecuteRawTransactionDto
            {
                RawTransaction = createRaw.RawTransaction,
                Signature = signature
            });

            rawTransactionResult.ShouldNotBeEmpty();
            var consensusAddress =
                (await Client.GetContractAddressByName(Hash.FromString("AElf.ContractNames.Consensus")))
                .GetFormatted();

            _testOutputHelper.WriteLine(rawTransactionResult);
            Assert.True(rawTransactionResult == $"\"{consensusAddress}\"");
        }

        [Fact]
        public async Task SendRawTransactionAsync_Test()
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
                    ["value"] = Hash.FromString("AElf.ContractNames.Token").Value.ToBase64()
                }.ToString(),
                RefBlockNumber = height,
                RefBlockHash = blockHash
            });
            createRaw.RawTransaction.ShouldNotBeEmpty();

            var transactionId = Hash.FromRawBytes(ByteArrayHelper.HexStringToByteArray(createRaw.RawTransaction));
            var signature = GetSignatureWith(PrivateKey, transactionId.ToByteArray()).ToHex();

            var rawTransactionResult = await Client.SendRawTransactionAsync(new SendRawTransactionInput
            {
                Transaction = createRaw.RawTransaction,
                Signature = signature,
                ReturnTransaction = true
            });

            Assert.True(rawTransactionResult != null);

            var result = JsonConvert.SerializeObject(rawTransactionResult, Formatting.Indented);
            _testOutputHelper.WriteLine(result);
        }

        [Fact]
        public async Task SendTransactionAsync_Test()
        {
            var toAddress = GenesisAddress;
            var methodName = ContractMethodName;
            var param = Hash.FromString("AElf.ContractNames.Vote");

            var transaction = await Client.GenerateTransaction(_address, toAddress, methodName, param);
            var txWithSign = Client.SignTransaction(PrivateKey, transaction);

            var result = await Client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            result.ShouldNotBeNull();
            _testOutputHelper.WriteLine(result.TransactionId);
        }

        [Fact]
        public async Task SendTransactionsAsync_Test()
        {
            var toAddress = GenesisAddress;
            var methodName = ContractMethodName;
            var param1 = Hash.FromString("AElf.ContractNames.Token");
            var param2 = Hash.FromString("AElf.ContractNames.Vote");

            var parameters = new List<Hash> {param1, param2};
            var sb = new StringBuilder();

            foreach (var param in parameters)
            {
                var tx = await Client.GenerateTransaction(_address, toAddress, methodName, param);
                var txWithSign = Client.SignTransaction(PrivateKey, tx);
                sb.Append(txWithSign.ToByteArray().ToHex() + ',');
            }

            var result1 = await Client.SendTransactionsAsync(new SendTransactionsInput
            {
                RawTransactions = sb.ToString().Substring(0, sb.Length - 1)
            });

            Assert.True(result1 != null);
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(result1));

        }

        [Fact]
        public async Task GetTransactionResultAsync_Test()
        {
            var firstBlockDto = await Client.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();

            var transactionResultDto = await Client.GetTransactionResultAsync(transactionId);
            Assert.True(transactionResultDto.Status == TransactionResultStatus.Mined.ToString().ToUpper());
        }

        [Fact]
        public async Task GetTransactionResultsAsync_Test()
        {
            var firstBlockDto = await Client.GetBlockByHeightAsync(1, true);
            var blockHash = firstBlockDto.BlockHash;

            var transactionResults = await Client.GetTransactionResultsAsync(blockHash, 0, 2);
            foreach (var transaction in transactionResults)
            {
                Assert.True(transaction.Status == TransactionResultStatus.Mined.ToString());
            }
        }

        [Fact]
        public async Task GetMerklePathByTransactionIdAsync_Test()
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
        public async Task GetChainIdAsync_Test()
        {
            var chainId = await Client.GetChainIdAsync();
            chainId.ShouldNotBeNull();
            chainId.ShouldBeOfType(typeof(int));

            _testOutputHelper.WriteLine(chainId.ToString());
        }

        [Fact]
        public async Task IsConnected_Test()
        {
            var isConnected = await Client.IsConnected();
            isConnected.ShouldBeTrue();
        }

        [Fact]
        public async Task GetGenesisContractAddressAsync_Test()
        {
            var genesisAddress = await Client.GetGenesisContractAddressAsync();
            genesisAddress.ShouldNotBeEmpty();

            var address = await Client.GetContractAddressByName(Hash.Empty);
            var genesisAddress2 = address.GetFormatted();
            Assert.True(genesisAddress == genesisAddress2);
        }

        [Fact]
        public async Task GetFormattedAddress_Test()
        {
            var result = await Client.GetFormattedAddress(AddressHelper.Base58StringToAddress(_address));
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
        public async Task GetAccountBalance_Test()
        {
            var tokenAddress = await Client.GetContractAddressByName(Hash.FromString("AElf.ContractNames.Token"));
            var methodName = "GetBalance";
            var param = new GetBalanceInput
            {
                Symbol = "ELF",
                Owner = new Proto.Address {Value = AddressHelper.Base58StringToAddress(_address).Value}
            };

            var transaction =
                await Client.GenerateTransaction(_address, tokenAddress.GetFormatted(), methodName, param);
            var txWithSign = Client.SignTransaction(PrivateKey, transaction);

            var transactionResult = await Client.ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });
            Assert.True(transactionResult != null);

            var balance = GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(transactionResult));
            _testOutputHelper.WriteLine($"Balance of {_address} = {balance.Balance} {balance.Symbol}");
        }

        [Fact]
        public async Task TransactionFee_Test()
        {
            var toAccount = Client.GenerateKeyPairInfo().Address;
            var toAddress = await Client.GetContractAddressByName(Hash.FromString("AElf.ContractNames.Token"));
            var methodName = "Transfer";
            var param = new TransferInput
            {
                To = new Proto.Address {Value = AddressHelper.Base58StringToAddress(toAccount).Value},
                Symbol = "ELF",
                Amount = 1000
            };

            var transaction = await Client.GenerateTransaction(_address, toAddress.GetFormatted(), methodName, param);
            var txWithSign = Client.SignTransaction(PrivateKey, transaction);

            var result = await Client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            result.ShouldNotBeNull();
            _testOutputHelper.WriteLine(result.TransactionId);

            await Task.Delay(4000);
            var transactionResult = await Client.GetTransactionResultAsync(result.TransactionId);
            var transactionFees = transactionResult.GetTransactionFees();
            transactionFees.First().Key.ShouldBe("ELF");
            transactionFees.First().Value.ShouldBe(25760000L);
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(transactionFees, Formatting.Indented));
        }

        [Fact]
        public async Task GetTransactionFee_Test()
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
                    }
                }
            };

            var transactionFees = transactionResultDto.GetTransactionFees();
            transactionFees.Count.ShouldBe(2);
            transactionFees["ELF"].ShouldBe(1000);
            transactionFees["READ"].ShouldBe(800);
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
}