using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web.Service
{
    public interface IChainAppService
    {
        Task<ChainStatusDto> GetChainStatusAsync();

        Task<byte[]> GetContractFileDescriptorSetAsync(string address);

        Task<RoundDto> GetCurrentRoundInformationAsync();

        Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync();
    }

    public partial class AElfWebService : IChainAppService
    {
        public async Task<ChainStatusDto> GetChainStatusAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetChainStatus);
            return await _httpService.GetResponseAsync<ChainStatusDto>(url);
        }

        public async Task<byte[]> GetContractFileDescriptorSetAsync(string address)
        {
            var url = GetRequestUrl(ApiMethods.GetContractFileDescriptorSet, address);
            return await _httpService.GetResponseAsync<byte[]>(url);
        }

        public async Task<RoundDto> GetCurrentRoundInformationAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetCurrentRoundInformation);
            return await _httpService.GetResponseAsync<RoundDto>(url);
        }

        public async Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetTaskQueueStatus);
            return await _httpService.GetResponseAsync<List<TaskQueueInfoDto>>(url);
        }
    }
}