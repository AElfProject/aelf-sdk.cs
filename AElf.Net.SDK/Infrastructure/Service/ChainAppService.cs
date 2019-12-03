using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Net.SDK.Infrastructure.Dto;

namespace AElf.Net.SDK.Infrastructure.Service
{
    public interface IChainAppService
    {
        Task<ChainStatusDto> GetChainStatusAsync();

        Task<byte[]> GetContractFileDescriptorSetAsync(string address);

        Task<RoundDto> GetCurrentRoundInformationAsync();

        Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync();

        Task<int> GetChainIdAsync();
    }

    public partial class AElfService : IChainAppService
    {
        public async Task<ChainStatusDto> GetChainStatusAsync()
        {
            var url = $"{RequestUrl}/api/blockChain/chainStatus";
            return await _httpService.GetResponseAsync<ChainStatusDto>(url);
        }

        public async Task<byte[]> GetContractFileDescriptorSetAsync(string address)
        {
            var url = $"{RequestUrl}/api/blockChain/contractFileDescriptorSet?address={address}";
            return await _httpService.GetResponseAsync<byte[]>(url);
        }

        public async Task<RoundDto> GetCurrentRoundInformationAsync()
        {
            var url = $"{RequestUrl}/api/blockChain/currentRoundInformation";
            return await _httpService.GetResponseAsync<RoundDto>(url);
        }

        public async Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync()
        {
            var url = $"{RequestUrl}/api/blockChain/taskQueueStatus";
            return await _httpService.GetResponseAsync<List<TaskQueueInfoDto>>(url);
        }

        public async Task<int> GetChainIdAsync()
        {
            var url = $"{RequestUrl}/api/blockChain/chainStatus";
            var statusDto = await _httpService.GetResponseAsync<ChainStatusDto>(url);
            var base58ChainId = statusDto.ChainId;
            var chainId = ChainHelper.ConvertBase58ToChainId(base58ChainId);

            return chainId;
        }
    }
}