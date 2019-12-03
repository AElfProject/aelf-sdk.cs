using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Net.SDK.Infrastructure.Dto;

namespace AElf.Net.SDK.Infrastructure.Service
{
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

    public partial class AElfService : ITransactionAppService
    {
        public async Task<TransactionPoolStatusOutput> GetTransactionPoolStatusAsync()
        {
            var url = $"{RequestUrl}/api/blockChain/transactionPoolStatus";
            return await _httpService.GetResponseAsync<TransactionPoolStatusOutput>(url);
        }

        public async Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input)
        {
            var url = $"{RequestUrl}/api/blockChain/executeTransaction";
            var parameters = new Dictionary<string, string>
            {
                {"RawTransaction", input.RawTransaction}
            };

            return await _httpService.PostResponseAsync<string>(url, parameters);
        }

        public async Task<string> ExecuteRawTransactionAsync(ExecuteRawTransactionDto input)
        {
            var url = $"{RequestUrl}/api/blockChain/executeRawTransaction";
            var parameters = new Dictionary<string, string>
            {
                {"RawTransaction", input.RawTransaction},
                {"Signature", input.Signature}
            };

            return await _httpService.PostResponseAsync<string>(url, parameters);
        }

        public async Task<CreateRawTransactionOutput> CreateRawTransactionAsync(CreateRawTransactionInput input)
        {
            var url = $"{RequestUrl}/api/blockChain/rawTransaction";
            var parameters = new Dictionary<string, string>
            {
                {"From", input.From},
                {"To", input.To},
                {"RefBlockNumber", input.RefBlockNumber.ToString()},
                {"RefBlockHash", input.RefBlockHash},
                {"MethodName", input.MethodName},
                {"Params", input.Params}
            };

            return await _httpService.PostResponseAsync<CreateRawTransactionOutput>(url, parameters);
        }

        public async Task<SendRawTransactionOutput> SendRawTransactionAsync(SendRawTransactionInput input)
        {
            var url = $"{RequestUrl}/api/blockChain/sendRawTransaction";
            var parameters = new Dictionary<string, string>
            {
                {"Transaction", input.Transaction},
                {"Signature", input.Signature},
                {"ReturnTransaction", input.ReturnTransaction ? "true" : "false"}
            };
            return await _httpService.PostResponseAsync<SendRawTransactionOutput>(url, parameters);
        }

        public async Task<SendTransactionOutput> SendTransactionAsync(SendTransactionInput input)
        {
            var url = $"{RequestUrl}/api/blockChain/sendTransaction";
            var parameters = new Dictionary<string, string>
            {
                {"RawTransaction", input.RawTransaction}
            };
            return await _httpService.PostResponseAsync<SendTransactionOutput>(url, parameters);
        }

        public async Task<string[]> SendTransactionsAsync(SendTransactionsInput input)
        {
            var url = $"{RequestUrl}/api/blockChain/sendTransactions";
            var parameters = new Dictionary<string, string>
            {
                {"RawTransactions", input.RawTransactions}
            };
            return await _httpService.PostResponseAsync<string[]>(url, parameters);
        }

        public async Task<TransactionResultDto> GetTransactionResultAsync(string transactionId)
        {
            var url = $"{RequestUrl}/api/blockChain/transactionResult?transactionId={transactionId}";
            return await _httpService.GetResponseAsync<TransactionResultDto>(url);
        }

        public async Task<List<TransactionResultDto>> GetTransactionResultsAsync(string blockHash, int offset = 0,
            int limit = 10)
        {
            var url = $"{RequestUrl}/api/blockChain/transactionResults?blockHash={blockHash}&offset={offset}&limit={limit}";
            return await _httpService.GetResponseAsync<List<TransactionResultDto>>(url);
        }

        public async Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId)
        {
            var url = $"{RequestUrl}/api/blockChain/merklePathByTransactionId?transactionId={transactionId}";
            return await _httpService.GetResponseAsync<MerklePathDto>(url);
        }
    }
}