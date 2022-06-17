using AElf.Types;

namespace AElf.Client.Abp.Genesis.DeployContract;

public interface IDeployContractService
{
    Task<Address?> DeployContract(string contractFileName);
}