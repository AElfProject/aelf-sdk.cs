using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web.Service
{
    public class ChainService : BaseService, IChainAppService
    {
        private readonly IHttpService _httpService;

        public ChainService(IHttpService httpService, string baseUrl)
        {
            _httpService = httpService;
            BaseUrl = FormatServiceUrl(baseUrl);
        }

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

        public async Task<RoundDto> GetRoundFromBase64Async(string base64Info)
        {
            var url = GetRequestUrl(ApiMethods.GetRoundFromBase64, base64Info);
            return await _httpService.GetResponseAsync<RoundDto>(url);
        }

        public async Task<RoundDto> GetCurrentRoundInformationAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetCurrentRoundInformation);
            return await _httpService.GetResponseAsync<RoundDto>(url);
        }

        public async Task<List<MiningSequenceDto>> GetMiningSequencesAsync(int count)
        {
            var url = GetRequestUrl(ApiMethods.GetMiningSequences, count);
            return await _httpService.GetResponseAsync<List<MiningSequenceDto>>(url);
        }
    }
}