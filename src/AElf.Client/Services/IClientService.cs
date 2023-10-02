using System.Threading.Tasks;
using AElf.Client.Model;
using AElf.Types;

namespace AElf.Client.Services
{
    
    public interface IClientService
    {
        Task<bool> IsConnectedAsync();
        Task<string> GetFormattedAddressAsync(Address address);
        Task<string?> GetGenesisContractAddressAsync();
        Task<Address> GetContractAddressByNameAsync(Hash contractNameHash);
        string GetAddressFromPubKey(string pubKey);
        KeyPairInfo GenerateKeyPairInfo();
    }
}
