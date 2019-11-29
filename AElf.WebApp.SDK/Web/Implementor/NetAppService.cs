using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.WebApp.SDK.Web.Dto;

namespace AElf.WebApp.SDK.Web.Service
{
    public class NetService : BaseService, INetAppService
    {
        private readonly IHttpService _httpService;

        public NetService(IHttpService httpService, string baseUrl)
        {
            _httpService = httpService;
            BaseUrl = FormatServiceUrl(baseUrl);
        }

        public async Task<bool> AddPeerAsync(AddPeerInput input)
        {
            var url = GetRequestUrl(ApiMethods.AddPeer);
            var parameters = new Dictionary<string, string>
            {
                {"address", input.Address}
            };

            return await _httpService.PostResponseAsync<bool>(url, parameters);
        }

        public async Task<bool> RemovePeerAsync(string address)
        {
            var url = GetRequestUrl(ApiMethods.RemovePeer, address);
            return await _httpService.DeleteResponseAsObjectAsync<bool>(url);
        }

        public async Task<List<PeerDto>> GetPeersAsync(bool withMetrics)
        {
            var url = GetRequestUrl(ApiMethods.GetPeers, withMetrics);
            return await _httpService.GetResponseAsync<List<PeerDto>>(url);
        }

        public async Task<NetworkInfoOutput> GetNetworkInfoAsync()
        {
            var url = GetRequestUrl(ApiMethods.GetNetworkInfo);
            return await _httpService.GetResponseAsync<NetworkInfoOutput>(url);
        }
    }
}