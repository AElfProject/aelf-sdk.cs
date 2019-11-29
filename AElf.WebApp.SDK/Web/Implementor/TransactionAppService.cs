using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web.Service
{
    public class TransactionService : BaseService, ITransactionAppService
    {
        private readonly IHttpService _httpService;

        public TransactionService(IHttpService httpService, string baseUrl)
        {
            _httpService = httpService;
            BaseUrl = FormatServiceUrl(baseUrl);
        }

        public async Task<TransactionPoolStatusOutput> GetTransactionPoolStatusAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetTransactionPoolStatus);
            return await _httpService.GetResponseAsync<TransactionPoolStatusOutput>(url);
        }

        public async Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input)
        {
            var url = GetRequestUrl(ApiMethods.ExecuteTransaction);
            var parameters = new Dictionary<string, string>
            {
                {"RawTransaction", input.RawTransaction}
            };

            return await _httpService.PostResponseAsync<string>(url, parameters);
        }

        public async Task<string> ExecuteRawTransactionAsync(ExecuteRawTransactionDto input)
        {
            var url = GetRequestUrl(ApiMethods.ExecuteRawTransaction);
            var parameters = new Dictionary<string, string>
            {
                {"RawTransaction", input.RawTransaction},
                {"Signature", input.Signature}
            };

            return await _httpService.PostResponseAsync<string>(url, parameters);
        }

        public async Task<CreateRawTransactionOutput> CreateRawTransactionAsync(CreateRawTransactionInput input)
        {
            var url = GetRequestUrl(ApiMethods.CreateRawTransaction);
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
            var url = GetRequestUrl(ApiMethods.SendRawTransaction);
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
            var url = GetRequestUrl(ApiMethods.SendTransaction);
            var parameters = new Dictionary<string, string>
            {
                {"RawTransaction", input.RawTransaction}
            };
            return await _httpService.PostResponseAsync<SendTransactionOutput>(url, parameters);
        }

        public async Task<string[]> SendTransactionsAsync(SendTransactionsInput input)
        {
            var url = GetRequestUrl(ApiMethods.SendTransactions);
            var parameters = new Dictionary<string, string>
            {
                {"RawTransactions", input.RawTransactions}
            };
            return await _httpService.PostResponseAsync<string[]>(url, parameters);
        }

        public async Task<TransactionResultDto> GetTransactionResultAsync(string transactionId)
        {
            var url = GetRequestUrl(ApiMethods.GetTransactionResult, transactionId);
            return await _httpService.GetResponseAsync<TransactionResultDto>(url);
        }

        public async Task<List<TransactionResultDto>> GetTransactionResultsAsync(string blockHash, int offset = 0,
            int limit = 10)
        {
            var url = GetRequestUrl(ApiMethods.GetTransactionResults, blockHash, offset, limit);
            return await _httpService.GetResponseAsync<List<TransactionResultDto>>(url);
        }

        public async Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId)
        {
            var url = GetRequestUrl(ApiMethods.GetMerklePathByTransactionId, transactionId);
            return await _httpService.GetResponseAsync<MerklePathDto>(url);
        }
    }
}