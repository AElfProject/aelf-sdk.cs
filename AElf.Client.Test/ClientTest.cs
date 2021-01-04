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
using Google.Protobuf.WellKnownTypes;
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
        private const string BaseUrl = "Http://127.0.0.1:8000";

        private string _genesisAddress;
        private string GenesisAddress => GetGenesisContractAddress();
        
        // example contract-method-name
        private string ContractMethodName => "GetContractAddressByName";

        // Address and privateKey of a node.
        private readonly string _address;
        private const string PrivateKey = "cd86ab6347d8e52bbbe8532141fc59ce596268143a308d1d40fedf385528b458";

        private readonly AElfClient _client;
        private readonly ITestOutputHelper _testOutputHelper;

        public ClientTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _client = new AElfClient(BaseUrl);
            
            // To get address from privateKey.s
            _address = _client.GetAddressFromPrivateKey(PrivateKey);
        }

        #region block

        [Fact]
        public async Task GetBlockHeightTest()
        {
            var height = await _client.GetBlockHeightAsync();
            height.ShouldBeGreaterThanOrEqualTo(1);
            _testOutputHelper.WriteLine(height.ToString());
        }

        [Fact]
        public async Task GetBlock_Test()
        {
            var height = await _client.GetBlockHeightAsync();
            var blockByHeight = await _client.GetBlockByHeightAsync(height);
            var blockByHash = await _client.GetBlockByHashAsync(blockByHeight.BlockHash);
            
            var blockByHeightSerialized =JsonConvert.SerializeObject(blockByHeight, Formatting.Indented);
            var blockByHashSerialized =JsonConvert.SerializeObject(blockByHash, Formatting.Indented);
            
            blockByHeightSerialized.ShouldBe(blockByHashSerialized);
            blockByHeight.Header.Height.ShouldBe(height);
            await VerifyBlockAsync(blockByHeight, false);
        }
        
        [Fact]
        public async Task GetBlockWithTransaction_Test()
        {
            var height = await _client.GetBlockHeightAsync();
            var blockByHeight = await _client.GetBlockByHeightAsync(height, true);
            var blockByHash = await _client.GetBlockByHashAsync(blockByHeight.BlockHash,true);
            
            var blockByHeightSerialized =JsonConvert.SerializeObject(blockByHeight, Formatting.Indented);
            var blockByHashSerialized =JsonConvert.SerializeObject(blockByHash, Formatting.Indented);
            
            blockByHeightSerialized.ShouldBe(blockByHashSerialized);
            blockByHeight.Header.Height.ShouldBe(height);
            await VerifyBlockAsync(blockByHeight, true);
        }

        private async Task VerifyBlockAsync(BlockDto block, bool withTransaction)
        {
            block.Header.PreviousBlockHash.ShouldNotBeNullOrWhiteSpace();
            block.Header.MerkleTreeRootOfTransactions.ShouldNotBeNullOrWhiteSpace();
            block.Header.MerkleTreeRootOfWorldState.ShouldNotBeNullOrWhiteSpace();
            block.Header.MerkleTreeRootOfTransactionState.ShouldNotBeNullOrWhiteSpace();
            block.Header.Extra.ShouldNotBeNullOrWhiteSpace();
            block.Header.ChainId.ShouldBe("AELF");
            block.Header.Bloom.ShouldNotBeNullOrWhiteSpace();
            block.Header.SignerPubkey.ShouldNotBeNullOrWhiteSpace();
            block.Header.Time.ShouldNotBe(new DateTime());
            block.Body.TransactionsCount.ShouldBeGreaterThan(0);

            if (withTransaction)
            {
                block.Body.Transactions.Count.ShouldBe(block.Body.TransactionsCount);
                block.Body.Transactions.ForEach(tx => tx.ShouldNotBeNullOrWhiteSpace());
            }
            else
            {
                block.Body.Transactions.Count.ShouldBe(0);
            }

            var previousBlock = await _client.GetBlockByHashAsync(block.Header.PreviousBlockHash);
            previousBlock.BlockHash.ShouldBe(block.Header.PreviousBlockHash);
            previousBlock.Header.Height.ShouldBe(block.Header.Height - 1);
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
            var chainStatusDto = await _client.GetChainStatusAsync();
            
            chainStatusDto.Branches.Count.ShouldBeGreaterThanOrEqualTo(1);
            chainStatusDto.Branches.First().Key.ShouldNotBeNullOrWhiteSpace();
            chainStatusDto.Branches.First().Value.ShouldBeGreaterThanOrEqualTo(1);
            chainStatusDto.ChainId.ShouldBe("AELF");
            chainStatusDto.BestChainHash.ShouldNotBeNullOrWhiteSpace();
            chainStatusDto.BestChainHeight.ShouldBeGreaterThanOrEqualTo(1);
            chainStatusDto.GenesisBlockHash.ShouldNotBeNullOrWhiteSpace();
            chainStatusDto.GenesisContractAddress.ShouldNotBeNullOrWhiteSpace();
            chainStatusDto.LongestChainHash.ShouldNotBeNullOrWhiteSpace();
            chainStatusDto.LongestChainHeight.ShouldBeGreaterThanOrEqualTo(1);
            chainStatusDto.LastIrreversibleBlockHash.ShouldNotBeNullOrWhiteSpace();
            chainStatusDto.LastIrreversibleBlockHeight.ShouldBeGreaterThanOrEqualTo(1);

            var longestChainBlock = await _client.GetBlockByHashAsync(chainStatusDto.LongestChainHash);
            longestChainBlock.Header.Height.ShouldBe(chainStatusDto.LongestChainHeight);

            var genesisBlock = await _client.GetBlockByHashAsync(chainStatusDto.GenesisBlockHash);
            genesisBlock.Header.Height.ShouldBe(1);

            var lastIrreversibleBlock = await _client.GetBlockByHashAsync(chainStatusDto.LastIrreversibleBlockHash);
            lastIrreversibleBlock.Header.Height.ShouldBe(chainStatusDto.LastIrreversibleBlockHeight);

            var bestChainBlock = await _client.GetBlockByHashAsync(chainStatusDto.BestChainHash);
            bestChainBlock.Header.Height.ShouldBe(chainStatusDto.BestChainHeight);

            var genesisContractAddress = await _client.GetGenesisContractAddressAsync();
            genesisContractAddress.ShouldBe(chainStatusDto.GenesisContractAddress);
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

            var addSuccess = await _client.AddPeerAsync(addressToAdd);
            addSuccess.ShouldBeTrue();
            _testOutputHelper.WriteLine($"Added ipAddress: {addressToAdd}");
        }

        [Fact(Skip = "Redo this later.")]
        public async Task RemovePeer_Test()
        {
            var peers = await _client.GetPeersAsync(false);
            peers.ShouldNotBeEmpty();

            var peerToRemoveAddress = peers[0].IpAddress;
            var removeSuccess = await _client.RemovePeerAsync(peerToRemoveAddress);
            Assert.True(removeSuccess);
            _testOutputHelper.WriteLine($"Removed ipAddress: {peerToRemoveAddress}");
        }

        [Fact]
        public async Task GetPeers_Test()
        {
            var peers = await _client.GetPeersAsync(false);
            Assert.True(peers != null);
            var peersInfo = JsonConvert.SerializeObject(peers, Formatting.Indented);
            _testOutputHelper.WriteLine(peersInfo);
        }

        [Fact]
        public async Task GetNetworkInfo_Test()
        {
            var netWorkInfo = await _client.GetNetworkInfoAsync();
            Assert.True(netWorkInfo != null);
            var info = JsonConvert.SerializeObject(netWorkInfo, Formatting.Indented);
            _testOutputHelper.WriteLine(info);
        }

        #endregion

        #region transaction

        [Fact]
        public async Task GetTaskQueueStatus_Test()
        {
            var taskQueueStatus = await _client.GetTaskQueueStatusAsync();
            taskQueueStatus.ShouldNotBeEmpty();

            var queueStatus = JsonConvert.SerializeObject(taskQueueStatus, Formatting.Indented);
            _testOutputHelper.WriteLine(queueStatus);
        }

        [Fact]
        public async Task GetTransactionPoolStatus_Test()
        {
            var poolStatus = await _client.GetTransactionPoolStatusAsync();
            Assert.True(poolStatus != null);

            var status = JsonConvert.SerializeObject(poolStatus, Formatting.Indented);
            _testOutputHelper.WriteLine(status);
        }

        [Fact]
        public async Task ExecuteTransaction_Test()
        {
            var toAddress = await _client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
            var methodName = "GetNativeTokenInfo";
            var param = new Empty();

            var transaction = await _client.GenerateTransactionAsync(_address, toAddress.ToBase58(), methodName, param);
            var txWithSign = _client.SignTransaction(PrivateKey, transaction);

            var transactionResult = await _client.ExecuteTransactionAsync(new ExecuteTransactionDto
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
            var status = await _client.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var createRaw = await _client.CreateRawTransactionAsync(new CreateRawTransactionInput
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
            var status = await _client.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var createRaw = await _client.CreateRawTransactionAsync(new CreateRawTransactionInput
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
            var rawTransactionResult = await _client.ExecuteRawTransactionAsync(new ExecuteRawTransactionDto
            {
                RawTransaction = createRaw.RawTransaction,
                Signature = signature
            });

            rawTransactionResult.ShouldNotBeEmpty();
            var consensusAddress =
                (await _client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Consensus")))
                .ToBase58();

            var addressResult = rawTransactionResult.Trim('"');
            _testOutputHelper.WriteLine(rawTransactionResult);
            addressResult.ShouldBe(consensusAddress);
        }

        [Fact]
        public async Task SendRawTransaction_Test()
        {
            var toAddress = GenesisAddress;
            var status = await _client.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var createRaw = await _client.CreateRawTransactionAsync(new CreateRawTransactionInput
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

            var rawTransactionResult = await _client.SendRawTransactionAsync(new SendRawTransactionInput
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
            var tokenContractAddress = await _client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
            var keyPair = _client.GenerateKeyPairInfo();
            var transaction = await CreateTransferTransactionAsync(keyPair.Address);

            var result = await _client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = transaction.ToByteArray().ToHex()
            });
            result.ShouldNotBeNull();
            result.TransactionId.ShouldNotBeNull();
            
            await Task.Delay(4000);

            var transactionResult = await _client.GetTransactionResultAsync(result.TransactionId);
            transactionResult.TransactionId.ShouldBe(result.TransactionId);
            transactionResult.Status.ShouldBe("MINED");
            transactionResult.Error.ShouldBeNullOrWhiteSpace();
            transactionResult.Logs.Length.ShouldBe(2);
            
            transactionResult.Logs[0].Address.ShouldBe(tokenContractAddress.ToBase58());
            transactionResult.Logs[0].Name.ShouldBe("TransactionFeeCharged");
            var feeCharged =
                TransactionFeeCharged.Parser.ParseFrom(
                    ByteString.FromBase64(transactionResult.Logs[0].NonIndexed));
            feeCharged.Symbol.ShouldBe("ELF");
            feeCharged.Amount.ShouldBeGreaterThan(0);
            
            transactionResult.Logs[1].Address.ShouldBe(tokenContractAddress.ToBase58());
            transactionResult.Logs[1].Name.ShouldBe("Transferred");
            var transferred =
                Transferred.Parser.ParseFrom(
                    ByteString.FromBase64(transactionResult.Logs[1].Indexed[0]));
            transferred.From.Value.ShouldBe(_client.GetBase58String(_address).Value);
            transferred =
                Transferred.Parser.ParseFrom(
                    ByteString.FromBase64(transactionResult.Logs[1].Indexed[1]));
            transferred.To.Value.ShouldBe(_client.GetBase58String(keyPair.Address).Value);
            transferred =
                Transferred.Parser.ParseFrom(
                    ByteString.FromBase64(transactionResult.Logs[1].Indexed[2]));
            transferred.Symbol.ShouldBe("ELF");
            transferred =
                Transferred.Parser.ParseFrom(
                    ByteString.FromBase64(transactionResult.Logs[1].NonIndexed));
            transferred.Amount.ShouldBe(1000000000);
            transferred.Memo.ShouldBe("transfer in test");
        }

        [Fact]
        public async Task SendFailedTransaction_Test()
        {
            var tokenContractAddress =
                await _client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
            var keyPair = _client.GenerateKeyPairInfo();
            var methodName = "Transfer";
            var param = new TransferInput
            {
                To = new Proto.Address {Value = Address.FromBase58(_address).Value},
                Symbol = "ELF",
                Amount = 1000000000,
                Memo = "transfer in test"
            };

            var transaction = await _client.GenerateTransactionAsync(keyPair.Address, tokenContractAddress.ToBase58(), methodName, param);
            var txWithSign = _client.SignTransaction(keyPair.PrivateKey, transaction);

            var result = await _client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });
            result.ShouldNotBeNull();
            result.TransactionId.ShouldNotBeNull();

            await Task.Delay(4000);

            var transactionResult = await _client.GetTransactionResultAsync(result.TransactionId);
            transactionResult.TransactionId.ShouldBe(result.TransactionId);
            transactionResult.Status.ShouldBe("NODEVALIDATIONFAILED");
            transactionResult.Error.ShouldBe("Pre-Error: Transaction fee not enough.");
        }

        [Fact]
        public async Task SendTransactions_Test()
        {
            var transactions = new List<string>();
            for (var i = 0; i < 2; i++)
            {
                var keyPair = _client.GenerateKeyPairInfo();
                var transaction = await CreateTransferTransactionAsync(keyPair.Address);
                transactions.Add(transaction.ToByteArray().ToHex());
            }

            var result = await _client.SendTransactionsAsync(new SendTransactionsInput
            {
                RawTransactions = string.Join(',',transactions)
            });
            
            await Task.Delay(4000);

            for (var i = 0; i < 2; i++)
            {
                var transactionResult = await _client.GetTransactionResultAsync(result[i]);
                transactionResult.Status.ShouldBe("MINED");
            }

        }

        [Fact]
        public async Task GetTransactionResult_Test()
        {
            var firstBlockDto = await _client.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();

            var transactionResultDto = await _client.GetTransactionResultAsync(transactionId);
            Assert.True(transactionResultDto.Status == TransactionResultStatus.Mined.ToString().ToUpper());
        }

        [Fact]
        public async Task GetTransactionResults_Test()
        {
            var firstBlockDto = await _client.GetBlockByHeightAsync(1, true);
            var blockHash = firstBlockDto.BlockHash;

            var transactionResults = await _client.GetTransactionResultsAsync(blockHash, 0, 2);
            foreach (var transaction in transactionResults)
            {
                Assert.True(transaction.Status == TransactionResultStatus.Mined.ToString().ToUpper());
            }
        }

        [Fact]
        public async Task GetMerklePathByTransactionId_Test()
        {
            var firstBlockDto = await _client.GetBlockByHeightAsync(1, true);
            var transactionId = firstBlockDto.Body.Transactions.FirstOrDefault();
            var merklePathDto = await _client.GetMerklePathByTransactionIdAsync(transactionId);
            Assert.True(merklePathDto != null);

            var merklePath = JsonConvert.SerializeObject(merklePathDto, Formatting.Indented);
            merklePath.ShouldNotBeEmpty();

            _testOutputHelper.WriteLine(merklePath);
        }

        [Fact]
        public async Task GetChainId_Test()
        {
            var chainId = await _client.GetChainIdAsync();
            chainId.ShouldBe(9992731);
        }

        [Fact]
        public async Task IsConnected_Test()
        {
            var isConnected = await _client.IsConnectedAsync();
            isConnected.ShouldBeTrue();
        }

        [Fact]
        public async Task GetGenesisContractAddress_Test()
        {
            var genesisAddress = await _client.GetGenesisContractAddressAsync();
            genesisAddress.ShouldNotBeEmpty();

            var address = await _client.GetContractAddressByNameAsync(Hash.Empty);
            var genesisAddress2 = address.ToBase58();
            Assert.True(genesisAddress == genesisAddress2);
        }

        [Fact]
        public async Task GetFormattedAddress_Test()
        {
            var result = await _client.GetFormattedAddressAsync(Address.FromBase58(_address));
            _testOutputHelper.WriteLine(result);
            Assert.True(result == $"ELF_{_address}_AELF");
        }

        [Fact]
        public void GetNewKeyPairInfo_Test()
        {
            var output = _client.GenerateKeyPairInfo();
            var addressOutput = JsonConvert.SerializeObject(output);
            _testOutputHelper.WriteLine(addressOutput);
        }

        [Fact]
        public async Task Transfer_Test()
        {
            var toAccount = _client.GenerateKeyPairInfo().Address;
            var toAddress = await _client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
            var methodName = "Transfer";
            var param = new TransferInput
            {
                To = new Proto.Address {Value = Address.FromBase58(toAccount).Value},
                Symbol = "ELF",
                Amount = 1000
            };

            var transaction = await _client.GenerateTransactionAsync(_address, toAddress.ToBase58(), methodName, param);
            var txWithSign = _client.SignTransaction(PrivateKey, transaction);

            var result = await _client.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            result.ShouldNotBeNull();
            _testOutputHelper.WriteLine(result.TransactionId);

            await Task.Delay(4000);
            var transactionResult = await _client.GetTransactionResultAsync(result.TransactionId);
            transactionResult.Status.ShouldBe(TransactionResultStatus.Mined.ToString().ToUpper());
            var transactionFees = transactionResult.GetTransactionFees();
            transactionFees.First().Key.ShouldBe("ELF");
            transactionFees.First().Value.ShouldBeGreaterThan(0L);
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(transactionFees, Formatting.Indented));

            var paramGetBalance = new GetBalanceInput
            {
                Symbol = "ELF",
                Owner = new Proto.Address {Value = Address.FromBase58(toAccount).Value}
            };

            var transactionGetBalance =
                await _client.GenerateTransactionAsync(_address, toAddress.ToBase58(), "GetBalance", paramGetBalance);
            var txWithSignGetBalance = _client.SignTransaction(PrivateKey, transactionGetBalance);

            var transactionGetBalanceResult = await _client.ExecuteTransactionAsync(new ExecuteTransactionDto
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

        #endregion

        #region private methods

        private string GetGenesisContractAddress()
        {
            if (_genesisAddress != null) return _genesisAddress;

            var statusDto = AsyncHelper.RunSync(_client.GetChainStatusAsync);
            _genesisAddress = statusDto.GenesisContractAddress;

            return _genesisAddress;
        }

        private ByteString GetSignatureWith(string privateKey, byte[] txData)
        {
            // Sign the hash
            var signature = CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(privateKey), txData);
            return ByteString.CopyFrom(signature);
        }

        private async Task<Transaction> CreateTransferTransactionAsync(string toAddress)
        {
            var tokenContractAddress = await _client.GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
            var methodName = "Transfer";
            var param = new TransferInput
            {
                To = new Proto.Address {Value = Address.FromBase58(toAddress).Value},
                Symbol = "ELF",
                Amount = 1000000000,
                Memo = "transfer in test"
            };

            var transaction = await _client.GenerateTransactionAsync(_address, tokenContractAddress.ToBase58(), methodName, param);
            var txWithSign = _client.SignTransaction(PrivateKey, transaction);

            return txWithSign;
        }

        #endregion
    }
}