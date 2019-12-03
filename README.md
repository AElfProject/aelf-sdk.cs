# AElf-Dotnet-SDK

DotNet Sdk for AElf.

## Introduction

This is a .NET SDK library, written in C#, used to communicate with the AElf API.

### Basic usage

``` c#
private const int TimeOut = 60;
private const int RetryTimes = 3;
private const string RequestUrl = "Http://127.0.0.1:8100";

var AElfClient = new AElfService(new HttpService(TimeOut, RetryTimes), RequestUrl);
```

### Interface

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

### Test

This module contains tests for all interface services provided by AElfClient. The usage of various services is described in detail in the test code.

##### Note

You need to run a local or remote AElf node to run the unit test successfully.