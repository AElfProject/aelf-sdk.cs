using System;
using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using Google.Protobuf;

namespace AElf.Client.Service
{
    public interface IClientService
    {
        Task<bool> IsConnected();
        Task<string> GetAccountFromPrivateKey(string privateKeyHex);
        Task<string> GetAccountFromPubKey(string pubKey);
        Task<string> GetPublicKey(string privateKeyHex);
        Task<string> GetGenesisContractAddressAsync();
        Task<Address> GetContractAddressByName(Hash contractNameHash, string privateKeyHex);
    }

    public partial class AelfClient : IClientService
    {
        private readonly IHttpService _httpService;
        private string BaseUrl { get; set; }

        public AelfClient(string baseUrl, int timeOut = 60, int retryTimes = 3)
        {
            _httpService = new HttpService(timeOut, retryTimes);
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Verify whether this sdk successfully connects the chain.
        /// </summary>
        /// <returns>IsConnected or not</returns>
        public async Task<bool> IsConnected()
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
        /// Get the account address through the private key.
        /// </summary>
        /// <param name="privateKeyHex"></param>
        /// <returns></returns>
        public Task<string> GetAccountFromPrivateKey(string privateKeyHex)
        {
            var address = Address.FromPublicKey(GetAElfKeyPair(privateKeyHex).PublicKey);
            return Task.FromResult(address.GetFormatted());
        }

        /// <summary>
        /// Get the account address through the public key.
        /// </summary>
        /// <param name="pubKey"></param>
        /// <returns>Account</returns>
        public Task<string> GetAccountFromPubKey(string pubKey)
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
        public async Task<Address> GetContractAddressByName(Hash contractNameHash, string privateKeyHex)
        {
            var from = await GetAccountFromPrivateKey(privateKeyHex);
            var to = await GetGenesisContractAddressAsync();
            var transaction = await GenerateTransaction(from, to, "GetContractAddressByName", contractNameHash);
            var txWithSig = await SignTransaction(privateKeyHex, transaction);

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
        public async Task<Transaction> GenerateTransaction(string from, string to,
            string methodName, IMessage input)
        {
            try
            {
                var chainStatus = await GetChainStatusAsync();
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

                return transaction;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Sign a transaction using private key.
        /// </summary>
        /// <param name="privateKeyHex"></param>
        /// <param name="transaction"></param>
        /// <returns>Transaction signed</returns>
        public Task<Transaction> SignTransaction(string privateKeyHex, Transaction transaction)
        {
            var transactionData = transaction.GetHash().ToByteArray();

            // Sign the hash
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var signature = CryptoHelper.SignWithPrivateKey(privateKey, transactionData);
            transaction.Signature = ByteString.CopyFrom(signature);

            return Task.FromResult(transaction);
        }

        public string GetFormattedAddress(Address address)
        {
            return address.GetFormatted();
        }

        public Address GetBase58String(string base58String)
        {
            return AddressHelper.Base58StringToAddress(base58String);
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

        #endregion
    }
}