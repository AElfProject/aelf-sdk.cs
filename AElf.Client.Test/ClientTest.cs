using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Types;
using AElf.Client.Dto;
using AElf.Client.Runtime;
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
        private const string PrivateKey = "09da44778f8db2e602fb484334f37df19e221c84c4582ce5b7770ccfbc3ddbef";

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

        [Fact]
        public async Task GetContractFileDescriptorSetAsync_Test()
        {
            var contractAddress = GenesisAddress;
            var fileDescriptorBytes = await Client.GetContractFileDescriptorSetAsync(contractAddress);
            var descriptorSet = FileDescriptorSet.Parser.ParseFrom(fileDescriptorBytes);
            descriptorSet.ShouldNotBeNull();
        }

        [Fact(Skip = "Redo this later.")]
        public async Task GetCurrentRoundInformationAsync_Test()
        {
            var webAppService = new AElfClient(BaseUrl, 60);
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
            var peers = await Client.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();
            Assert.True(peers.Count >= 1);

            // remove the last ipAddress
            var peerToRemoveAddress = peers[^1].IpAddress;
            var removeSuccess = await Client.RemovePeerAsync(peerToRemoveAddress);
            removeSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");

            // add removed ipAddress
            var addressToAdd = peerToRemoveAddress;

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
            var transactions = new List<Transaction>();

            foreach (var param in parameters)
            {
                var tx = await Client.GenerateTransaction(_address, toAddress, methodName, param);
                var txWithSign = Client.SignTransaction(PrivateKey, tx);

                transactions.Add(txWithSign);
            }

            var result1 = await Client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transactions[0].ToByteArray().ToHex()
            });

            Assert.True(result1 != null);

            var result2 = await Client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transactions[1].ToByteArray().ToHex()
            });

            Assert.True(result2 != null);
            _testOutputHelper.WriteLine(result1.TransactionId + "\r\n" + result2.TransactionId);
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