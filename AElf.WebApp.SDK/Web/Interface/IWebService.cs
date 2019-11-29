using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web
{
    public interface INetAppService
    {
        Task<bool> AddPeerAsync(AddPeerInput input);

        Task<bool> RemovePeerAsync(string address);

        Task<List<PeerDto>> GetPeersAsync(bool withMetrics);

        Task<NetworkInfoOutput> GetNetworkInfoAsync();
    }

    public interface IBlockAppService
    {
        Task<long> GetBlockHeightAsync();

        Task<BlockDto> GetBlockAsync(string blockHash, bool includeTransactions = false);

        Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false);
    }

    public interface IChainAppService
    {
        Task<ChainStatusDto> GetChainStatusAsync();

        Task<byte[]> GetContractFileDescriptorSetAsync(string address);

        Task<RoundDto> GetCurrentRoundInformationAsync();

        Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync();
    }

    public interface ITransactionAppService
    {
        Task<TransactionPoolStatusOutput> GetTransactionPoolStatusAsync();

        Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input);

        Task<string> ExecuteRawTransactionAsync(ExecuteRawTransactionDto input);

        Task<CreateRawTransactionOutput> CreateRawTransactionAsync(CreateRawTransactionInput input);

        Task<SendRawTransactionOutput> SendRawTransactionAsync(SendRawTransactionInput input);

        Task<SendTransactionOutput> SendTransactionAsync(SendTransactionInput input);

        Task<string[]> SendTransactionsAsync(SendTransactionsInput input);

        Task<TransactionResultDto> GetTransactionResultAsync(string transactionId);

        Task<List<TransactionResultDto>> GetTransactionResultsAsync(string blockHash, int offset = 0,
            int limit = 10);

        Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId);
    }
}