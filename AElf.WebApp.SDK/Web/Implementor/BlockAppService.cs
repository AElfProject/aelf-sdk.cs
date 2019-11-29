using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web.Service
{
    public class BlockService : BaseService, IBlockAppService
    {
        private readonly IHttpService _httpService;

        public BlockService(IHttpService httpService, string baseUrl)
        {
            _httpService = httpService;
            BaseUrl = FormatServiceUrl(baseUrl);
        }

        public async Task<long> GetBlockHeightAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetBlockHeight);
            return await _httpService.GetResponseAsync<long>(url);
        }

        public async Task<BlockDto> GetBlockAsync(string blockHash, bool includeTransactions = false)
        {
            var url = GetRequestUrl(ApiMethods.GetBlockByHash, blockHash, includeTransactions);
            return await _httpService.GetResponseAsync<BlockDto>(url);
        }

        public async Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false)
        {
            var url = GetRequestUrl(ApiMethods.GetBlockByHeight, blockHeight, includeTransactions);
            return await _httpService.GetResponseAsync<BlockDto>(url);
        }
    }
}