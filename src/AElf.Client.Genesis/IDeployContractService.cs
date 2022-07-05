using AElf.Types;

namespace AElf.Client.Genesis;

public interface IDeployContractService
{
    Task<Address?> DeployContract(string contractFileName);
}