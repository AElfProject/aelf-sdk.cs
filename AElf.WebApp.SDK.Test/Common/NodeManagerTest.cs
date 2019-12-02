using System.Threading.Tasks;
using AElf.WebApp.SDK.Common;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AElf.WebApp.SDK.Test
{
    public class NodeManagerTest
    {
        private INodeManager Manager { get; }
        private readonly ITestOutputHelper _testOutputHelper;

        private const string Url = "127.0.0.1:8001";
        private const int RetryTimes = 3;
        private const int TimeOut = 60;

        // Info of a running node.
        // TODO init the account
        private const string Account = "2bWwpsN9WSc4iKJPHYL4EZX3nfxVY7XLadecnNMar1GdSb4hJz";
        private readonly string _privateKey = "09da44778f8db2e602fb484334f37df19e221c84c4582ce5b7770ccfbc3ddbef";

        public NodeManagerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Manager = new NodeManager(Url);
        }

        [Fact]
        public async Task GetChainIdAsync_Test()
        {
            var chainId = await Manager.GetChainIdAsync();
            chainId.ShouldNotBeNull();
            _testOutputHelper.WriteLine(chainId.ToString());
        }

        [Fact]
        public async Task IsConnected_Test()
        {
            var isConnected = await Manager.IsConnected();
            isConnected.ShouldBeTrue();
        }

        [Fact]
        public async Task GetAccountFromPrivateKeyAsync_Test()
        {
            var address = await Manager.GetAccountFromPrivateKeyAsync(_privateKey);
            Assert.True(address == Account);
        }
        
        [Fact]
        public async Task GetAccountFromPubKeyAsync_Test()
        {
            var pubKey = await Manager.GetPublicKey(_privateKey);
            var address = await Manager.GetAccountFromPubKeyAsync(pubKey);
            Assert.True(address == Account);
        }

        [Fact]
        public async Task GetGenesisContractAddressAsync_Test()
        {
            var genesisAddress = await Manager.GetGenesisContractAddressAsync();
            genesisAddress.ShouldNotBeEmpty();

            var address = await Manager.GetContractAddressByName(NameProvider.Genesis, _privateKey);
            var genesisAddress2 = address.GetFormatted();
            Assert.True(genesisAddress == genesisAddress2);
        }
    }
}