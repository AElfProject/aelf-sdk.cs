# AElf-Client

## Introduction

This is a C# client library, used to communicate with the [AElf](https://github.com/AElfProject/AElf)  API.

### Basic usage

``` c#
private const int TimeOut = 60;
private const int RetryTimes = 3;
private const string BaseUrl = "Http://127.0.0.1:8100";

// get client instance
var AElfClient = new AElfService(new HttpService(TimeOut, RetryTimes), BaseUrl);
var height = await AElfClient.GetBlockHeightAsync();
```

### Interface

The interface methods can be easily available by instance "AElfClient" shown in basic usage. The following is a list of input parameters and type of output for each method. Check out the [Web api reference](https://docs.aelf.io/v/dev/reference) for detailed Interface description.

#### IBlockAppService

```c#
 Task<long> GetBlockHeightAsync();

 Task<BlockDto> GetBlockByHashAsync(string blockHash, bool includeTransactions = false);

 Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false);
```

#### IChainAppService

```c#
 Task<ChainStatusDto> GetChainStatusAsync();

 Task<byte[]> GetContractFileDescriptorSetAsync(string address);

 Task<RoundDto> GetCurrentRoundInformationAsync();

 Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync();

 Task<int> GetChainIdAsync();
```

#### INetAppService

```c#
 Task<bool> AddPeerAsync(AddPeerInput input);

 Task<bool> RemovePeerAsync(string address);

 Task<List<PeerDto>> GetPeersAsync(bool withMetrics);

 Task<NetworkInfoOutput> GetNetworkInfoAsync();
```

#### ITransactionAppService

```c#
Task<TransactionPoolStatusOutput> GetTransactionPoolStatusAsync();

Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input);

Task<string> ExecuteRawTransactionAsync(ExecuteRawTransactionDto input);

Task<CreateRawTransactionOutput> CreateRawTransactionAsync(CreateRawTransactionInput input);

Task<SendRawTransactionOutput> SendRawTransactionAsync(SendRawTransactionInput input);

Task<SendTransactionOutput> SendTransactionAsync(SendTransactionInput input);

Task<string[]> SendTransactionsAsync(SendTransactionsInput input);

Task<TransactionResultDto> GetTransactionResultAsync(string transactionId);

Task<List<TransactionResultDto>> GetTransactionResultsAsync(string blockHash, int offset = 0,int limit = 10);

Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId);
```

#### IClientService

```c#
 Task<bool> IsConnected();

 Task<string> GetAccountFromPrivateKey(string privateKeyHex);

 Task<string> GetAccountFromPubKey(string pubKey);

 Task<string> GetPublicKey(string privateKeyHex);

 Task<string> GetGenesisContractAddressAsync();

 Task<Address> GetContractAddressByName(Hash contractNameHash, string privateKeyHex);
```

### Test

This module contains tests for all services provided by AElfClient. In this module, you can see how to properly use services provided by AElfClient. 

### Note

You need to run a local or remote AElf node to run the unit test successfully.If you're not familiar with how to run a node or multiple nodes, please see [Running a node](https://docs.aelf.io/v/dev/main/main/run-node) /[Running multiple nodes](https://docs.aelf.io/v/dev/main/main/multi-nodes) for more information.