using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Net.SDK.Infrastructure.Dto;

namespace AElf.Net.SDK.Infrastructure.Service
{
    public interface INetAppService
    {
        Task<bool> AddPeerAsync(AddPeerInput input);

        Task<bool> RemovePeerAsync(string address);

        Task<List<PeerDto>> GetPeersAsync(bool withMetrics);

        Task<NetworkInfoOutput> GetNetworkInfoAsync();
    }
    
    public partial class AElfService : INetAppService
    {
        public async Task<bool> AddPeerAsync(AddPeerInput input)
        { 
            var url = $"{RequestUrl}/api/net/peer";
            var parameters = new Dictionary<string, string>
            {
                {"address", input.Address}
            };

            return await _httpService.PostResponseAsync<bool>(url, parameters);
        }

        public async Task<bool> RemovePeerAsync(string address)
        {
            var url = $"{RequestUrl}/api/net/peer?address={address}";
            return await _httpService.DeleteResponseAsObjectAsync<bool>(url);
        }

        public async Task<List<PeerDto>> GetPeersAsync(bool withMetrics)
        {
            var url = $"{RequestUrl}/api/net/peers?withMetrics={withMetrics}";
            return await _httpService.GetResponseAsync<List<PeerDto>>(url);
        }

        public async Task<NetworkInfoOutput> GetNetworkInfoAsync()
        {
            var url = $"{RequestUrl}/api/net/networkInfo";
            return await _httpService.GetResponseAsync<NetworkInfoOutput>(url);
        }
    }
}