using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Types;
using AElf.WebApp.SDK.Web;
using AElf.WebApp.SDK.Web.Dto;
using AElf.WebApp.SDK.Web.Extension;
using AElf.WebApp.SDK.Web.Service;
using AElfSdk;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp.Threading;
using Xunit;
using Xunit.Abstractions;

namespace AElf.WebApp.SDK.Test
{
    public class WebAppTest
    {
        private const string Url = "127.0.0.1:8001";
        private const int RetryTimes = 3;
        private const int TimeOut = 60;

        private string _genesisAddress;
        private string GenesisAddress => GetGenesisContractAddress();
        private string ContractMethodName => "GetContractAddressByName";

        // Info of a running node.
        // TODO init the account
        private const string Account = "2bWwpsN9WSc4iKJPHYL4EZX3nfxVY7XLadecnNMar1GdSb4hJz";
        private const string PrivateKey = "09da44778f8db2e602fb484334f37df19e221c84c4582ce5b7770ccfbc3ddbef";

        private AElfWebService WebService { get; }
        private IHttpService _httpService;
        private readonly ITestOutputHelper _testOutputHelper;

        public WebAppTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            WebService = AElfWebAppClient.GetClientByUrl(Url);
        }

        #region block

        [Fact]
        public async Task GetBlockHeightTest()
        {
            var height = await WebService.GetBlockHeightAsync();
            height.ShouldNotBeNull();
            _testOutputHelper.WriteLine(height.ToString());
        }

        [Fact]
        public async Task GetBlock_ByHeightAsyncTest()
        {
            var height = await WebService.GetBlockHeightAsync();
            var blockDto = await WebService.GetBlockByHeightAsync(height);
            Assert.True(blockDto != null);

            var block = JsonConvert.SerializeObject(blockDto, Formatting.Indented);
            _testOutputHelper.WriteLine(block);
        }

        [Fact]
        public async Task GetBlockByHeight_Failed_Test()
        {
            const int heightNotExist = int.MaxValue;
            _httpService = new HttpService(TimeOut, RetryTimes);
            var errorResponse = await _httpService.GetResponseAsync<WebAppErrorResponse>(
                $"http://{Url}/api/blockChain/blockByHeight?blockHeight={heightNotExist}&includeTransactions=false",
                expectedStatusCode: HttpStatusCode.Forbidden);
            errorResponse.Error.Code.ShouldBe(Error.NotFound.ToString());
            errorResponse.Error.Message.ShouldBe(Error.Message[Error.NotFound]);
            var str = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
            _testOutputHelper.WriteLine(str);
        }

        [Fact]
        public async Task GetBlockAsync_Success_Test()
        {
            var firstBlockDto = await WebService.GetBlockByHeightAsync(1);
            var genesisHash = firstBlockDto.BlockHash;

            var blockDto = await WebService.GetBlockAsync(genesisHash, true);
            Assert.True(blockDto != null);

            var block = JsonConvert.SerializeObject(blockDto, Formatting.Indented);
            _testOutputHelper.WriteLine(block);
        }

        #endregion

        #region chain

        [Fact]
        public async Task GetChainStatusAsync_Test()
        {
            var chainStatusDto = await WebService.GetChainStatusAsync();
            Assert.True(chainStatusDto != null);

            var chainStatus = JsonConvert.SerializeObject(chainStatusDto, Formatting.Indented);
            _testOutputHelper.WriteLine(chainStatus);
        }

        [Fact]
        public async Task GetContractFileDescriptorSetAsync_Test()
        {
            var contractAddress = GenesisAddress;
            var fileDescriptorBytes = await WebService.GetContractFileDescriptorSetAsync(contractAddress);
            var descriptorSet = FileDescriptorSet.Parser.ParseFrom(fileDescriptorBytes);
            descriptorSet.ShouldNotBeNull();
        }

        [Fact(Skip = "Redo this later.")]
        public async Task GetCurrentRoundInformationAsync_Test()
        {
            var webAppService = AElfWebAppClient.GetClientByUrl(Url, retryTimes: 10, timeout:60);
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
            var peers = await WebService.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();
            Assert.True(peers.Count >= 1);

            // remove the last ipAddress
            var peerToRemoveAddress = peers[^1].IpAddress;
            var removeSuccess = await WebService.RemovePeerAsync(peerToRemoveAddress);
            removeSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");

            // add removed ipAddress
            var peerInput = new AddPeerInput
            {
                Address = peerToRemoveAddress
            };
            var addSuccess = await WebService.AddPeerAsync(peerInput);
            addSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Added ipAddress: {peerInput.Address}");
        }

        [Fact(Skip = "Redo this later.")]
        public async Task RemovePeerAsync_Test()
        {
            var peers = await WebService.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();

            var peerToRemoveAddress = peers[0].IpAddress;
            var removeSuccess = await WebService.RemovePeerAsync(peerToRemoveAddress);
            Assert.True(removeSuccess);
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");
        }

        [Fact]
        public async Task GetPeersAsync_Test()
        {
            var peers = await WebService.GetPeersAsync(false);
            Assert.True(peers != null);
            var peersInfo = JsonConvert.SerializeObject(peers, Formatting.Indented);
            _testOutputHelper.WriteLine(peersInfo);
        }

        [Fact]
        public async Task GetNetworkInfoAsync_Test()
        {
            var netWorkInfo = await WebService.GetNetworkInfoAsync();
            Assert.True(netWorkInfo != null);
            var info = JsonConvert.SerializeObject(netWorkInfo, Formatting.Indented);
            _testOutputHelper.WriteLine(info);
        }

        #endregion

        #region transaction

        [Fact]
        public async Task GetTaskQueueStatusAsync_Test()
        {
            var taskQueueStatus = await WebService.GetTaskQueueStatusAsync();
            taskQueueStatus.ShouldNotBeEmpty();

            var queueStatus = JsonConvert.SerializeObject(taskQueueStatus, Formatting.Indented);
            _testOutputHelper.WriteLine(queueStatus);
        }

        [Fact]
        public async Task GetTransactionPoolStatusAsync_Test()
        {
            var poolStatus = await WebService.GetTransactionPoolStatusAsync();
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

            var transaction = await GenerateTransactionAsync(Account, toAddress, methodName, param);
            var signature = GetSignatureWith(PrivateKey, transaction.GetHash().ToByteArray());
            transaction.Signature = ByteString.CopyFrom(signature.ToByteArray());

            var transactionResult = await WebService.ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = transaction.ToByteArray().ToHex()
            });
            Assert.True(transactionResult != null);
            _testOutputHelper.WriteLine(transactionResult);
        }

        [Fact]
        public async Task ExecuteRawTransactionAsync_Test()
        {
            var address = GenesisAddress;
            var height = await WebService.GetBlockHeightAsync();
            var block = await WebService.GetBlockByHeightAsync(height);

            var createRaw = await WebService.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = Account,
                To = address,
                MethodName = ContractMethodName,
                Params = new JObject
                {
                    ["value"] = Hash.FromString("AElf.ContractNames.Consensus").Value.ToBase64()
                }.ToString(),
                RefBlockNumber = height,
                RefBlockHash = block.BlockHash
            });

            var transactionId = Hash.FromRawBytes(ByteArrayHelper.HexStringToByteArray(createRaw.RawTransaction));
            var signature = GetSignatureWith(PrivateKey, transactionId.ToByteArray()).ToHex();

            var rawTransactionResult = await WebService.ExecuteRawTransactionAsync(new ExecuteRawTransactionDto
            {
                RawTransaction = createRaw.RawTransaction,
                Signature = signature
            });

            rawTransactionResult.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task CreateRawTransactionAsync_Test()
        {
            var address = GenesisAddress;
            var height = await WebService.GetBlockHeightAsync();
            var block = await WebService.GetBlockByHeightAsync(height);

            var createRaw = await WebService.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = Account,
                To = address,
                MethodName = ContractMethodName,
                Params = new JObject
                {
                    ["value"] = Hash.FromString("AElf.ContractNames.Token").Value.ToBase64()
                }.ToString(),
                RefBlockNumber = height,
                RefBlockHash = block.BlockHash
            });

            createRaw.RawTransaction.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SendRawTransactionAsync_Test()
        {
            var toAddress = GenesisAddress;
            var height = await WebService.GetBlockHeightAsync();
            var block = await WebService.GetBlockByHeightAsync(height);

            var createRaw = await WebService.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = Account,
                To = toAddress,
                MethodName = ContractMethodName,
                Params = new JObject
                {
                    ["value"] = Hash.FromString("AElf.ContractNames.Token").Value.ToBase64()
                }.ToString(),
                RefBlockNumber = height,
                RefBlockHash = block.BlockHash
            });
            createRaw.RawTransaction.ShouldNotBeEmpty();

            var transactionId = Hash.FromRawBytes(ByteArrayHelper.HexStringToByteArray(createRaw.RawTransaction));
            var signature = GetSignatureWith(PrivateKey, transactionId.ToByteArray()).ToHex();

            var rawTransactionResult = await WebService.SendRawTransactionAsync(new SendRawTransactionInput
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
            var param = Hash.FromString("AElf.ContractNames.Token");

            var transaction = await GenerateTransactionAsync(Account, toAddress, methodName, param);
            var signature = GetSignatureWith(PrivateKey, transaction.GetHash().ToByteArray());
            transaction.Signature = ByteString.CopyFrom(signature.ToByteArray());

            var transactionResult = await WebService.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transaction.ToByteArray().ToHex()
            });

            transactionResult.ShouldNotBeNull();
            _testOutputHelper.WriteLine(transactionResult.TransactionId);
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
                var tx = await GenerateTransactionAsync(Account, toAddress, methodName, param);
                var signature = GetSignatureWith(PrivateKey, tx.GetHash().ToByteArray());
                tx.Signature = ByteString.CopyFrom(signature.ToByteArray());

                transactions.Add(tx);
            }

            transactions.Count.ShouldBe(2);
            var result1 = await WebService.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transactions[0].ToByteArray().ToHex()
            });

            Assert.True(result1 != null);

            var result2 = await WebService.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transactions[1].ToByteArray().ToHex()
            });

            Assert.True(result2 != null);
        }

        [Fact]
        public async Task GetTransactionResultAsync_Test()
        {
            var firstBlockDto = await WebService.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();

            var transactionResultDto = await WebService.GetTransactionResultAsync(transactionId);
            Assert.True(transactionResultDto.Status.ToTransactionResultStatus() == TransactionResultStatus.Mined);
        }

        [Fact]
        public async Task GetTransactionResultsAsync_Test()
        {
            var firstBlockDto = await WebService.GetBlockByHeightAsync(1, true);
            var blockHash = firstBlockDto.BlockHash;

            var transactionResults = await WebService.GetTransactionResultsAsync(blockHash, 0, 2);
            foreach (var transaction in transactionResults)
            {
                Assert.True(transaction.Status.ToTransactionResultStatus() == TransactionResultStatus.Mined);
            }
        }

        [Fact]
        public async Task GetMerklePathByTransactionIdAsync_Test()
        {
            var firstBlockDto = await WebService.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();
            var merklePathDto = await WebService.GetMerklePathByTransactionIdAsync(transactionId);
            Assert.True(merklePathDto != null);

            var merklePath = JsonConvert.SerializeObject(merklePathDto, Formatting.Indented);
            merklePath.ShouldNotBeEmpty();
            
            _testOutputHelper.WriteLine(merklePath);
        }

        [Fact]
        public async Task GetChainIdAsync_Test()
        {
            var chainId = await WebService.GetChainIdAsync();
            chainId.ShouldNotBeNull();
            chainId.ShouldBeOfType(typeof(int));
            
            _testOutputHelper.WriteLine(chainId.ToString());
        }

        #endregion

        #region private methods

        private string GetGenesisContractAddress()
        {
            if (_genesisAddress != null) return _genesisAddress;

            var statusDto = AsyncHelper.RunSync(WebService.GetChainStatusAsync);
            _genesisAddress = statusDto.GenesisContractAddress;

            return _genesisAddress;
        }

        private ByteString GetSignatureWith(string privateKey, byte[] txData)
        {
            // Sign the hash
            var signature = CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(privateKey), txData);
            return ByteString.CopyFrom(signature);
        }

        private async Task<Transaction> GenerateTransactionAsync(string from, string to, string methodName,
            IMessage input)
        {
            var height = await WebService.GetBlockHeightAsync();
            var block = await WebService.GetBlockByHeightAsync(height);
            var blockHash = HashHelper.HexStringToHash(block.BlockHash);

            var transaction = new Transaction
            {
                From = AddressHelper.Base58StringToAddress(from),
                To = AddressHelper.Base58StringToAddress(to),
                MethodName = methodName,
                Params = input.ToByteString(),
                RefBlockNumber = height,
                RefBlockPrefix = ByteString.CopyFrom(blockHash.Value.Take(4).ToArray())
            };

            return transaction;
        }
        
        #endregion
    }
}