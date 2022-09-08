using AElf.Types;

namespace AElf.Client.Genesis;

public interface IDeployContractService
{
    Task<Address?> DeployContractAsync(string contractFileName);
}