using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Types;
using AElf.Client.Dto;
using AElf.Client.Service;
using AElf.Runtime.CSharp;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp.Threading;
using Xunit;
using Xunit.Abstractions;

namespace AElf.Client.Test
{
    public class ClientTest
    {
        private const string BaseUrl = "Http://127.0.0.1:8001";
        private const int RetryTimes = 3;
        private const int TimeOut = 60;

        private string _genesisAddress;
        private string GenesisAddress => GetGenesisContractAddress();
        private string ContractMethodName => "GetContractAddressByName";

        // Info of a running node.
        private readonly string _account;
        private const string PrivateKey = "09da44778f8db2e602fb484334f37df19e221c84c4582ce5b7770ccfbc3ddbef";

        private AElfService AElfClient { get; }
        private readonly IHttpService _httpService;
        private readonly ITestOutputHelper _testOutputHelper;

        public ClientTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _httpService = new HttpService(TimeOut, RetryTimes);
            AElfClient = new AElfService(_httpService, BaseUrl);
            
            // To get account's address from privateKey.
            _account = AsyncHelper.RunSync(() => AElfClient.GetAccountFromPrivateKey(PrivateKey));
        }

        #region block

        [Fact]
        public async Task GetBlockHeightTest()
        {
            var height = await AElfClient.GetBlockHeightAsync();
            height.ShouldNotBeNull();
            _testOutputHelper.WriteLine(height.ToString());
        }

        [Fact]
        public async Task GetBlock_ByHeightAsyncTest()
        {
            var height = await AElfClient.GetBlockHeightAsync();
            var blockDto = await AElfClient.GetBlockByHeightAsync(height);
            Assert.True(blockDto != null);

            var block = JsonConvert.SerializeObject(blockDto, Formatting.Indented);
            _testOutputHelper.WriteLine(block);
        }

        [Fact]
        public async Task GetBlockByHeight_Failed_Test()
        {
            const int heightNotExist = int.MaxValue;
            var errorResponse = await _httpService.GetResponseAsync<WebAppErrorResponse>(
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
            var chainStatusDto = await AElfClient.GetChainStatusAsync();
            var genesisHash = chainStatusDto.GenesisBlockHash;

            var blockDto = await AElfClient.GetBlockByHashAsync(genesisHash, true);
            Assert.True(blockDto != null);

            var block = JsonConvert.SerializeObject(blockDto, Formatting.Indented);
            _testOutputHelper.WriteLine(block);
        }

        #endregion

        #region chain

        [Fact]
        public async Task GetChainStatusAsync_Test()
        {
            var chainStatusDto = await AElfClient.GetChainStatusAsync();
            Assert.True(chainStatusDto != null);

            var chainStatus = JsonConvert.SerializeObject(chainStatusDto, Formatting.Indented);
            _testOutputHelper.WriteLine(chainStatus);
        }

        [Fact]
        public async Task GetContractFileDescriptorSetAsync_Test()
        {
            var contractAddress = GenesisAddress;
            var fileDescriptorBytes = await AElfClient.GetContractFileDescriptorSetAsync(contractAddress);
            var descriptorSet = FileDescriptorSet.Parser.ParseFrom(fileDescriptorBytes);
            descriptorSet.ShouldNotBeNull();
        }

        [Fact(Skip = "Redo this later.")]
        public async Task GetCurrentRoundInformationAsync_Test()
        {
            var httpService = new HttpService(60, 5);
            var webAppService = new AElfService(httpService, BaseUrl);
            var roundDto = await webAppService.GetCurrentRoundInformationAsync();
            roundDto.ShouldNotBeNull();

            var currentRoundInformation = JsonConvert.SerializeObject(roundDto);
            _testOutputHelper.WriteLine(currentRoundInformation);
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
            var peers = await AElfClient.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();
            Assert.True(peers.Count >= 1);

            // remove the last ipAddress
            var peerToRemoveAddress = peers[^1].IpAddress;
            var removeSuccess = await AElfClient.RemovePeerAsync(peerToRemoveAddress);
            removeSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");

            // add removed ipAddress
            var peerInput = new AddPeerInput
            {
                Address = peerToRemoveAddress
            };
            var addSuccess = await AElfClient.AddPeerAsync(peerInput);
            addSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Added ipAddress: {peerInput.Address}");
        }

        [Fact(Skip = "Redo this later.")]
        public async Task RemovePeerAsync_Test()
        {
            var peers = await AElfClient.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();

            var peerToRemoveAddress = peers[0].IpAddress;
            var removeSuccess = await AElfClient.RemovePeerAsync(peerToRemoveAddress);
            Assert.True(removeSuccess);
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");
        }

        [Fact]
        public async Task GetPeersAsync_Test()
        {
            var peers = await AElfClient.GetPeersAsync(false);
            Assert.True(peers != null);
            var peersInfo = JsonConvert.SerializeObject(peers, Formatting.Indented);
            _testOutputHelper.WriteLine(peersInfo);
        }

        [Fact]
        public async Task GetNetworkInfoAsync_Test()
        {
            var netWorkInfo = await AElfClient.GetNetworkInfoAsync();
            Assert.True(netWorkInfo != null);
            var info = JsonConvert.SerializeObject(netWorkInfo, Formatting.Indented);
            _testOutputHelper.WriteLine(info);
        }

        #endregion

        #region transaction

        [Fact]
        public async Task GetTaskQueueStatusAsync_Test()
        {
            var taskQueueStatus = await AElfClient.GetTaskQueueStatusAsync();
            taskQueueStatus.ShouldNotBeEmpty();

            var queueStatus = JsonConvert.SerializeObject(taskQueueStatus, Formatting.Indented);
            _testOutputHelper.WriteLine(queueStatus);
        }

        [Fact]
        public async Task GetTransactionPoolStatusAsync_Test()
        {
            var poolStatus = await AElfClient.GetTransactionPoolStatusAsync();
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

            var transaction = await AElfClient.GenerateTransaction(_account, toAddress, methodName, param);
            var txWithSign = await AElfClient.SignTransaction(PrivateKey, transaction);

            var transactionResult = await AElfClient.ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });
            Assert.True(transactionResult != null);

            var addressResult = Address.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(transactionResult));
            var address = await AElfClient.GetContractAddressByName(param, PrivateKey);
            Assert.True(address == addressResult);
        }

        [Fact]
        public async Task CreateRawTransactionAsync_Test()
        {
            var address = GenesisAddress;
            var status = await AElfClient.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var createRaw = await AElfClient.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = _account,
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
            var status = await AElfClient.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var createRaw = await AElfClient.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = _account,
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
            var rawTransactionResult = await AElfClient.ExecuteRawTransactionAsync(new ExecuteRawTransactionDto
            {
                RawTransaction = createRaw.RawTransaction,
                Signature = signature
            });

            rawTransactionResult.ShouldNotBeEmpty();
            var consensusAddress =
                (await AElfClient.GetContractAddressByName(Hash.FromString("AElf.ContractNames.Consensus"), PrivateKey))
                .GetFormatted();

            Assert.True(rawTransactionResult == $"\"{consensusAddress}\"");
        }

        [Fact]
        public async Task SendRawTransactionAsync_Test()
        {
            var toAddress = GenesisAddress;
            var status = await AElfClient.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var createRaw = await AElfClient.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = _account,
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

            var rawTransactionResult = await AElfClient.SendRawTransactionAsync(new SendRawTransactionInput
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

            var transaction = await AElfClient.GenerateTransaction(_account, toAddress, methodName, param);
            var txWithSign = await AElfClient.SignTransaction(PrivateKey, transaction);

            var result = await AElfClient.SendTransactionAsync(new SendTransactionInput
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
            var transactions = new List<Transaction>();

            foreach (var param in parameters)
            {
                var tx = await AElfClient.GenerateTransaction(_account, toAddress, methodName, param);
                var txWithSign = await AElfClient.SignTransaction(PrivateKey, tx);

                transactions.Add(txWithSign);
            }

            var result1 = await AElfClient.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transactions[0].ToByteArray().ToHex()
            });

            Assert.True(result1 != null);

            var result2 = await AElfClient.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transactions[1].ToByteArray().ToHex()
            });

            Assert.True(result2 != null);
            _testOutputHelper.WriteLine(result1.TransactionId + "\r\n" + result2.TransactionId);
        }

        [Fact]
        public async Task GetTransactionResultAsync_Test()
        {
            var firstBlockDto = await AElfClient.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();

            var transactionResultDto = await AElfClient.GetTransactionResultAsync(transactionId);
            Assert.True(transactionResultDto.Status == TransactionResultStatus.Mined.ToString().ToUpper());
        }

        [Fact]
        public async Task GetTransactionResultsAsync_Test()
        {
            var firstBlockDto = await AElfClient.GetBlockByHeightAsync(1, true);
            var blockHash = firstBlockDto.BlockHash;

            var transactionResults = await AElfClient.GetTransactionResultsAsync(blockHash, 0, 2);
            foreach (var transaction in transactionResults)
            {
                Assert.True(transaction.Status == TransactionResultStatus.Mined.ToString());
            }
        }

        [Fact]
        public async Task GetMerklePathByTransactionIdAsync_Test()
        {
            var firstBlockDto = await AElfClient.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();
            var merklePathDto = await AElfClient.GetMerklePathByTransactionIdAsync(transactionId);
            Assert.True(merklePathDto != null);

            var merklePath = JsonConvert.SerializeObject(merklePathDto, Formatting.Indented);
            merklePath.ShouldNotBeEmpty();

            _testOutputHelper.WriteLine(merklePath);
        }

        [Fact]
        public async Task GetChainIdAsync_Test()
        {
            var chainId = await AElfClient.GetChainIdAsync();
            chainId.ShouldNotBeNull();
            chainId.ShouldBeOfType(typeof(int));

            _testOutputHelper.WriteLine(chainId.ToString());
        }

        [Fact]
        public async Task IsConnected_Test()
        {
            var isConnected = await AElfClient.IsConnected();
            isConnected.ShouldBeTrue();
        }

        [Fact]
        public async Task GetAccountFromPubKeyAsync_Test()
        {
            var pubKey = await AElfClient.GetPublicKey(PrivateKey);
            var address = await AElfClient.GetAccountFromPubKey(pubKey);
            Assert.True(address == _account);
        }

        [Fact]
        public async Task GetGenesisContractAddressAsync_Test()
        {
            var genesisAddress = await AElfClient.GetGenesisContractAddressAsync();
            genesisAddress.ShouldNotBeEmpty();

            var address = await AElfClient.GetContractAddressByName(Hash.Empty, PrivateKey);
            var genesisAddress2 = address.GetFormatted();
            Assert.True(genesisAddress == genesisAddress2);
        }

        #endregion

        #region private methods

        private string GetGenesisContractAddress()
        {
            if (_genesisAddress != null) return _genesisAddress;

            var statusDto = AsyncHelper.RunSync(AElfClient.GetChainStatusAsync);
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