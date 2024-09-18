using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Client.Dto;

namespace AElf.Client
{
    
    public interface IChainAppService
    {
        Task<ChainStatusDto> GetChainStatusAsync(string? baseUrl = null);
    
        Task<byte[]> GetContractFileDescriptorSetAsync(string? address);
    
        Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync();
    
        Task<int> GetChainIdAsync();
    }
}
