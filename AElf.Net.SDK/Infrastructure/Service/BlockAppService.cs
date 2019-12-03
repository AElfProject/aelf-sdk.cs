using System.Threading.Tasks;
using AElf.Net.SDK.Infrastructure.Dto;

namespace AElf.Net.SDK.Infrastructure.Service
{
    public interface IBlockAppService
    {
        Task<long> GetBlockHeightAsync();

        Task<BlockDto> GetBlockByHashAsync(string blockHash, bool includeTransactions = false);

        Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false);
    }

    public partial class AElfService : IBlockAppService
    {
        public async Task<long> GetBlockHeightAsync()
        {
            var url = $"{RequestUrl}/api/blockChain/blockHeight";
            return await _httpService.GetResponseAsync<long>(url);
        }

        public async Task<BlockDto> GetBlockByHashAsync(string blockHash, bool includeTransactions = false)
        {
            var url = $"{RequestUrl}/api/blockChain/block?blockHash={blockHash}&includeTransactions={includeTransactions}";
            return await _httpService.GetResponseAsync<BlockDto>(url);
        }

        public async Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false)
        {
            var url = $"{RequestUrl}/api/blockChain/blockByHeight?blockHeight={blockHeight}&includeTransactions={includeTransactions}";
            return await _httpService.GetResponseAsync<BlockDto>(url);
        }
    }
}