using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web.Service
{
    public class WebAppService : IWebAppService
    {
        private readonly INetAppService _netAppService;
        private readonly IBlockAppService _blockAppService;
        private readonly ITransactionAppService _transactionAppService;
        private readonly IChainAppService _chainAppService;

        public WebAppService(IChainAppService chainAppService, INetAppService netAppService,
            IBlockAppService blockAppService, ITransactionAppService transactionAppService)
        {
            _chainAppService = chainAppService;
            _netAppService = netAppService;
            _blockAppService = blockAppService;
            _transactionAppService = transactionAppService;
        }

        public async Task<bool> AddPeerAsync(AddPeerInput input)
        {
            return await _netAppService.AddPeerAsync(input);
        }

        public async Task<bool> RemovePeerAsync(string address)
        {
            return await _netAppService.RemovePeerAsync(address);
        }

        public async Task<List<PeerDto>> GetPeersAsync(bool withMetrics)
        {
            return await _netAppService.GetPeersAsync(withMetrics);
        }

        public async Task<NetworkInfoOutput> GetNetworkInfoAsync()
        {
            return await _netAppService.GetNetworkInfoAsync();
        }

        public async Task<long> GetBlockHeightAsync()
        {
            return await _blockAppService.GetBlockHeightAsync();
        }

        public async Task<BlockDto> GetBlockAsync(string blockHash, bool includeTransactions = false)
        {
            return await _blockAppService.GetBlockAsync(blockHash, includeTransactions);
        }

        public async Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false)
        {
            return await _blockAppService.GetBlockByHeightAsync(blockHeight, includeTransactions);
        }

        public async Task<BlockStateDto> GetBlockStateAsync(string blockHash)
        {
            return await _blockAppService.GetBlockStateAsync(blockHash);
        }

        public async Task<RoundDto> GetCurrentRoundInformationAsync()
        {
            return await _chainAppService.GetCurrentRoundInformationAsync();
        }

        public async Task<ChainStatusDto> GetChainStatusAsync()
        {
            return await _chainAppService.GetChainStatusAsync();
        }

        public async Task<byte[]> GetContractFileDescriptorSetAsync(string address)
        {
            return await _chainAppService.GetContractFileDescriptorSetAsync(address);
        }

        public async Task<RoundDto> GetRoundFromBase64Async(string base64Info)
        {
            return await _chainAppService.GetRoundFromBase64Async(base64Info);
        }

        public async Task<List<MiningSequenceDto>> GetMiningSequencesAsync(int count)
        {
            return await _chainAppService.GetMiningSequencesAsync(count);
        }

        public async Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync()
        {
            return await _transactionAppService.GetTaskQueueStatusAsync();
        }

        public async Task<TransactionPoolStatusOutput> GetTransactionPoolStatusAsync()
        {
            return await _transactionAppService.GetTransactionPoolStatusAsync();
        }

        public async Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input)
        {
            return await _transactionAppService.ExecuteTransactionAsync(input);
        }

        public async Task<string> ExecuteRawTransactionAsync(ExecuteRawTransactionDto input)
        {
            return await _transactionAppService.ExecuteRawTransactionAsync(input);
        }

        public async Task<CreateRawTransactionOutput> CreateRawTransactionAsync(CreateRawTransactionInput input)
        {
            return await _transactionAppService.CreateRawTransactionAsync(input);
        }

        public async Task<SendRawTransactionOutput> SendRawTransactionAsync(SendRawTransactionInput input)
        {
            return await _transactionAppService.SendRawTransactionAsync(input);
        }

        public async Task<SendTransactionOutput> SendTransactionAsync(SendTransactionInput input)
        {
            return await _transactionAppService.SendTransactionAsync(input);
        }

        public async Task<string[]> SendTransactionsAsync(SendTransactionsInput input)
        {
            return await _transactionAppService.SendTransactionsAsync(input);
        }

        public async Task<TransactionResultDto> GetTransactionResultAsync(string transactionId)
        {
            return await _transactionAppService.GetTransactionResultAsync(transactionId);
        }

        public async Task<List<TransactionResultDto>> GetTransactionResultsAsync(string blockHash, int offset = 0,
            int limit = 10)
        {
            return await _transactionAppService.GetTransactionResultsAsync(blockHash, offset, limit);
        }

        public async Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId)
        {
            return await _transactionAppService.GetMerklePathByTransactionIdAsync(transactionId);
        }
    }
}