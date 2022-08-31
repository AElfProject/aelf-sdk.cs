using AElf.Client.Dto;

namespace AElf.Client;

public interface IChainAppService
{
    Task<ChainStatusDto> GetChainStatusAsync();

    Task<byte[]> GetContractFileDescriptorSetAsync(string? address);

    Task<List<TaskQueueInfoDto>> GetTaskQueueStatusAsync();

    Task<int> GetChainIdAsync();
}