using System.Threading.Tasks;
using AElf.Types;
using AElf.WebApp.SDK.Web.Service;

namespace AElf.WebApp.SDK.Common
{
    public interface INodeManager
    {
        Task<int> GetChainIdAsync();
        Task<bool> IsConnected();
        Task<string> GetAccountFromPrivateKeyAsync(string privateKeyHex);
        Task<string> GetAccountFromPubKeyAsync(string pubKey);
        Task<string> GetPublicKey(string privateKeyHex);
        Task<string> GetGenesisContractAddressAsync();
        Task<Address> GetContractAddressByName(NameProvider contractName, string privateKeyHex);
    }
}