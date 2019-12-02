using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using AElf.WebApp.SDK.Web;
using AElf.WebApp.SDK.Web.Dto;
using AElf.WebApp.SDK.Web.Service;
using Google.Protobuf;

namespace AElf.WebApp.SDK.Common
{
    public class NodeManager : INodeManager
    {
        #region properties

        private int _chainId;
        private string _baseUrl;
        private string _genesisAddress;

        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }

        public int ChainId
        {
            get => _chainId;
            set => _chainId = value;
        }

        public static Dictionary<NameProvider, Hash> ContractNameDict => InitializeSystemContractsName();

        private readonly Dictionary<NameProvider, Address> _systemContractAddresses =
            new Dictionary<NameProvider, Address>();

        #endregion

        private AElfWebService AElfWebService { get; set; }

        public NodeManager(string baseUrl)
        {
            _baseUrl = baseUrl;
            AElfWebService = AElfWebAppClient.GetClientByUrl(baseUrl);
        }

        public async Task UpdateApiUrl(string url)
        {
            _baseUrl = url;
            AElfWebService = AElfWebAppClient.GetClientByUrl(url);
            _chainId = await GetChainIdAsync();
        }

        public async Task<bool> IsConnected()
        {
            try
            {
                var chainStatus = await AElfWebService.GetChainStatusAsync();
                return chainStatus != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> GetChainIdAsync()
        {
            var chainId = await AElfWebService.GetChainIdAsync();
            _chainId = chainId;

            return _chainId;
        }

        public async Task<string> GetGenesisContractAddressAsync()
        {
            if (_genesisAddress != null) return _genesisAddress;

            var statusDto = await AElfWebService.GetChainStatusAsync();
            _genesisAddress = statusDto.GenesisContractAddress;

            return _genesisAddress;
        }

        public Transaction BuildSignedTransaction(string from, string to,
            string methodName, IMessage input)
        {
            try
            {
                var chainStatus = AElfWebService.GetChainStatusAsync().Result;
                var transaction = new Transaction
                {
                    From = AddressHelper.Base58StringToAddress(from),
                    To = AddressHelper.Base58StringToAddress(to),
                    MethodName = methodName,
                    Params = input.ToByteString(),
                    RefBlockNumber = chainStatus.BestChainHeight,
                    RefBlockPrefix = ByteString.CopyFrom(HashHelper.HexStringToHash(chainStatus.BestChainHash).Value
                        .Take(4).ToArray())
                };

                var privateKeyHex = "";
                var signedTransaction = SignTransaction(privateKeyHex, transaction);

                return signedTransaction;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Transaction SignTransaction(string privateKeyHex, Transaction transaction)
        {
            var transactionData = transaction.GetHash().ToByteArray();

            // Sign the hash
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var signature = CryptoHelper.SignWithPrivateKey(privateKey, transactionData);
            transaction.Signature = ByteString.CopyFrom(signature);

            return transaction;
        }

        public async Task<Address> GetContractAddressByName(NameProvider contractName, string privateKeyHex)
        {
            if (_systemContractAddresses.ContainsKey(contractName))
                return _systemContractAddresses[contractName];

            if (contractName == NameProvider.Genesis)
            {
                var genesisAddress = AddressHelper.Base58StringToAddress(await GetGenesisContractAddressAsync());
                _systemContractAddresses[contractName] = genesisAddress;
                return genesisAddress;
            }

            var hash = ContractNameDict[contractName];
            var from = await GetAccountFromPrivateKeyAsync(privateKeyHex);
            var to = await GetGenesisContractAddressAsync();
            var txWithSig = BuildSignedTransaction(from, to, "GetContractAddressByName", hash);

            var response = await AElfWebService.ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSig.ToByteArray().ToHex()
            });
            var byteArray = ByteArrayHelper.HexStringToByteArray(response);
            var address = Address.Parser.ParseFrom(byteArray);

            _systemContractAddresses[contractName] = address;
            return address;
        }

        public Task<string> GetAccountFromPrivateKeyAsync(string privateKeyHex)
        {
            var address = Address.FromPublicKey(GetAElfKeyPair(privateKeyHex).PublicKey);
            return Task.FromResult(address.GetFormatted());
        }

        public Task<string> GetAccountFromPubKeyAsync(string pubKey)
        {
            var publicKey = ByteArrayHelper.HexStringToByteArray(pubKey);
            var address = Address.FromPublicKey(publicKey);
            return Task.FromResult(address.GetFormatted());
        }

        public Task<string> GetPublicKey(string privateKeyHex)
        {
            var keyPair = GetAElfKeyPair(privateKeyHex);
            return Task.FromResult(keyPair.PublicKey.ToHex());
        }

        #region private method

        private ECKeyPair GetAElfKeyPair(string privateKeyHex)
        {
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var keyPair = CryptoHelper.FromPrivateKey(privateKey);

            return keyPair;
        }

        private static Dictionary<NameProvider, Hash> InitializeSystemContractsName()
        {
            var dic = new Dictionary<NameProvider, Hash>
            {
                {NameProvider.Genesis, Hash.Empty},
                {NameProvider.Election, Hash.FromString("AElf.ContractNames.Election")},
                {NameProvider.Profit, Hash.FromString("AElf.ContractNames.Profit")},
                {NameProvider.Vote, Hash.FromString("AElf.ContractNames.Vote")},
                {NameProvider.Treasury, Hash.FromString("AElf.ContractNames.Treasury")},
                {NameProvider.Token, Hash.FromString("AElf.ContractNames.Token")},
                {NameProvider.TokenConverter, Hash.FromString("AElf.ContractNames.TokenConverter")},
                {NameProvider.Consensus, Hash.FromString("AElf.ContractNames.Consensus")},
                {NameProvider.ParliamentAuth, Hash.FromString("AElf.ContractNames.Parliament")},
                {NameProvider.CrossChain, Hash.FromString("AElf.ContractNames.CrossChain")},
                {NameProvider.AssociationAuth, Hash.FromString("AElf.ContractNames.AssociationAuth")},
                {NameProvider.Configuration, Hash.FromString("AElf.ContractNames.Configuration")},
                {NameProvider.ReferendumAuth, Hash.FromString("AElf.ContractNames.ReferendumAuth")}
            };

            return dic;
        }

        #endregion
    }
}