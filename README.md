# AElf-Client

BRANCH | AZURE PIPELINES                                                                                                                                                                                                        | TESTS                                                                                                                                                                                    | CODE COVERAGE
-------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------
MASTER   | [![Build Status](https://dev.azure.com/AElfProject/aelf-sdk.cs/_apis/build/status/AElfProject.aelf-sdk.cs?branchName=master)](https://dev.azure.com/AElfProject/aelf-sdk.cs/_build/latest?definitionId=14&branchName=master) | [![Test Status](https://img.shields.io/azure-devops/tests/AElfProject/aelf-sdk.cs/14/master)](https://dev.azure.com/AElfProject/aelf-sdk.cs/_build/latest?definitionId=14&branchName=master) | [![codecov](https://codecov.io/gh/AElfProject/aelf-sdk.cs/branch/master/graph/badge.svg?token=mBrO9ZNFAS)](https://codecov.io/gh/AElfProject/aelf-sdk.cs)
DEV    | [![Build Status](https://dev.azure.com/AElfProject/aelf-sdk.cs/_apis/build/status/AElfProject.aelf-sdk.cs?branchName=dev)](https://dev.azure.com/AElfProject/aelf-sdk.cs/_build/latest?definitionId=14&branchName=dev)       | [![Test Status](https://img.shields.io/azure-devops/tests/AElfProject/aelf-sdk.cs/14/dev)](https://dev.azure.com/AElfProject/aelf-sdk.cs/_build/latest?definitionId=14&branchName=dev)       | [![codecov](https://codecov.io/gh/AElfProject/aelf-sdk.cs/branch/dev/graph/badge.svg?token=mBrO9ZNFAS)](https://codecov.io/gh/AElfProject/aelf-sdk.cs)


## Introduction

This is a C# client library, used to communicate with the [AElf](https://github.com/AElfProject/AElf)  API.

### Getting Started

You should build the "AElf.Client" project first to get files defined in protos, which will be generated in the directory named "Protobuf/Generated".

### Basic usage

``` c#
private const string BaseUrl = "Http://127.0.0.1:8100";

// get client instance
AElfClient aelfClient = new AElfClient(BaseUrl);
var height = await aelfClient.GetBlockHeightAsync();
```

### Interface

Interface methods can be easily available by the instance "aelfClient" shown in basic usage. The following is a list of input parameters and output for each method. Check out the [Web api reference](https://docs.aelf.io/v/dev/reference) for detailed Interface description.

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
 Task<bool> AddPeerAsync(string address);

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

 Task<string> GetFormattedAddress(Address address);

 Task<string> GetAddressFromPubKey(string pubKey);

 Task<string> GetGenesisContractAddressAsync();

 Task<Address> GetContractAddressByName(Hash contractNameHash);
```

### Test

This module contains tests for all services provided by AElfClient. You can see how to properly use services provided by AElfClient here.

You need to firstly set necessary parameters to make sure tests can run successfully.

1. Set baseUrl to your target url.

   ```c#
   private const string BaseUrl = "Http://127.0.0.1:8001";
   ```

2. Give a valid privateKey of a node.

   ```c#
   private const string PrivateKey = "09da44778f8db2e602fb484334f37df19e221c84c4582ce5b7770ccfbc3ddbef";
   ```

### Note

You need to run a local or remote AElf node to run the unit test successfully. If you're not familiar with how to run a node or multiple nodes, please see [Running a node](https://docs.aelf.io/v/dev/main/main/run-node) / [Running multiple nodes](https://docs.aelf.io/v/dev/main/main/multi-nodes) for more information.