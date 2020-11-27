using System;
using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Client.Model;
using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Address = AElf.Types.Address;
using Hash = AElf.Types.Hash;

namespace AElf.Client.Service
{
    public interface IClientService
    {
        Task<bool> IsConnectedAsync();
        Task<string> GetFormattedAddressAsync(Address address);
        Task<string> GetGenesisContractAddressAsync();
        Task<Address> GetContractAddressByNameAsync(Hash contractNameHash);
        string GetAddressFromPubKey(string pubKey);
        KeyPairInfo GenerateKeyPairInfo();
    }

    public partial class AElfClient : IClientService
    {
        private readonly IHttpService _httpService;
        private string _baseUrl;

        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }

        private const string ExamplePrivateKey = "09da44778f8db2e602fb484334f37df19e221c84c4582ce5b7770ccfbc3ddbef";

        public AElfClient(string baseUrl, int timeOut = 60)
        {
            _httpService = new HttpService(timeOut);
            _baseUrl = baseUrl;
            
        }

        /// <summary>
        /// Verify whether this sdk successfully connects the chain.
        /// </summary>
        /// <returns>IsConnected or not</returns>
        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                var chainStatus = await GetChainStatusAsync();
                return chainStatus != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the address of genesis contract.
        /// </summary>
        /// <returns>Address</returns>
        public async Task<string> GetGenesisContractAddressAsync()
        {
            var statusDto = await GetChainStatusAsync();
            var genesisAddress = statusDto.GenesisContractAddress;

            return genesisAddress;
        }

        /// <summary>
        /// Get address of a contract by given contractNameHash.
        /// </summary>
        /// <param name="contractNameHash"></param>
        /// <param name="privateKeyHex"></param>
        /// <returns>Address</returns>
        public async Task<Address> GetContractAddressByNameAsync(Hash contractNameHash)
        {
            var from = GetAddressFromPrivateKey(ExamplePrivateKey);
            var to = await GetGenesisContractAddressAsync();
            var transaction = await GenerateTransactionAsync(from, to, "GetContractAddressByName", contractNameHash);
            var txWithSig = SignTransaction(ExamplePrivateKey, transaction);

            var response = await ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSig.ToByteArray().ToHex()
            });
            var byteArray = ByteArrayHelper.HexStringToByteArray(response);
            var address = Address.Parser.ParseFrom(byteArray);

            return address;
        }

        /// <summary>
        /// Build a transaction from the input parameters.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="methodName"></param>
        /// <param name="input"></param>
        /// <returns>Transaction unsigned</returns>
        public async Task<Transaction> GenerateTransactionAsync(string from, string to,
            string methodName, IMessage input)
        {
            try
            {
                AssertValidAddress(from, to);
                var chainStatus = await GetChainStatusAsync();
                var transaction = new Transaction
                {
                    From = Address.FromBase58(from),
                    To = Address.FromBase58(to),
                    MethodName = methodName,
                    Params = input.ToByteString(),
                    RefBlockNumber = chainStatus.BestChainHeight,
                    RefBlockPrefix = ByteString.CopyFrom(Hash.LoadFromHex(chainStatus.BestChainHash).Value
                        .Take(4).ToArray())
                };

                return transaction;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Convert the Address to the displayed string：symbol_base58-string_base58-string-chain-id
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<string> GetFormattedAddressAsync(Address address)
        {
            var tokenContractAddress = await GetContractAddressByNameAsync(HashHelper.ComputeFrom("AElf.ContractNames.Token"));
            var fromAddress = GetAddressFromPrivateKey(ExamplePrivateKey);
            var toAddress = tokenContractAddress.ToBase58();
            var methodName = "GetPrimaryTokenSymbol";
            var param = new Empty();

            var transaction = await GenerateTransactionAsync(fromAddress, toAddress, methodName, param);
            var txWithSign = SignTransaction(ExamplePrivateKey, transaction);

            var result = await ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            var symbol = StringValue.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
            var chainIdString = (await GetChainStatusAsync()).ChainId;

            return $"{symbol.Value}_{address.ToBase58()}_{chainIdString}";
        }

        /// <summary>
        /// Sign a transaction using private key.
        /// </summary>
        /// <param name="privateKeyHex"></param>
        /// <param name="transaction"></param>
        /// <returns>Transaction signed</returns>
        public Transaction SignTransaction(string privateKeyHex, Transaction transaction)
        {
            var transactionData = transaction.GetHash().ToByteArray();

            // Sign the hash
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var signature = CryptoHelper.SignWithPrivateKey(privateKey, transactionData);
            transaction.Signature = ByteString.CopyFrom(signature);

            return transaction;
        }

        /// <summary>
        /// Get the account address through the public key.
        /// </summary>
        /// <param name="pubKey"></param>
        /// <returns>Account</returns>
        public string GetAddressFromPubKey(string pubKey)
        {
            var publicKey = ByteArrayHelper.HexStringToByteArray(pubKey);
            var address = Address.FromPublicKey(publicKey);
            return address.ToBase58();
        }
        
        /// <summary>
        /// Get the account address through the private key.
        /// </summary>
        /// <param name="privateKeyHex"></param>
        /// <returns></returns>
        public string GetAddressFromPrivateKey(string privateKeyHex)
        {
            var address = Address.FromPublicKey(GetAElfKeyPair(privateKeyHex).PublicKey);
            return address.ToBase58();
        }

        public Address GetBase58String(string base58String)
        {
            return Address.FromBase58(base58String);
        }

        public KeyPairInfo GenerateKeyPairInfo()
        {
            var keyPair = CryptoHelper.GenerateKeyPair();
            var privateKey = keyPair.PrivateKey.ToHex();
            var publicKey = keyPair.PublicKey.ToHex();
            var address = GetAddressFromPrivateKey(privateKey);

            return new KeyPairInfo
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Address = address
            };
        }

        #region private methods

        private ECKeyPair GetAElfKeyPair(string privateKeyHex)
        {
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var keyPair = CryptoHelper.FromPrivateKey(privateKey);

            return keyPair;
        }

        private string GetRequestUrl(string baseUrl, string relativeUrl)
        {
            return new Uri(new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), relativeUrl).ToString();
        }

        private void AssertValidAddress(params string[] addresses)
        {
            try
            {
                foreach (var address in addresses)
                {
                    Types.Address.FromBase58(address);
                }
            }
            catch (Exception)
            {
                throw new AElfClientException(Error.Message[Error.InvalidAddress]);
            }
        }

        private void AssertValidHash(params string[] hashes)
        {
            try
            {
                foreach (var hash in hashes)
                {
                    Hash.LoadFromHex(hash);
                }
            }
            catch (Exception)
            {
                throw new AElfClientException(Error.Message[Error.InvalidBlockHash]);
            }
        }

        private void AssertValidTransactionId(params string[] transactionIds)
        {
            try
            {
                foreach (var transactionId in transactionIds)
                {
                    Hash.LoadFromHex(transactionId);
                }
            }
            catch (Exception)
            {
                throw new AElfClientException(Error.Message[Error.InvalidTransactionId]);
            }
        }

        #endregion
    }
}