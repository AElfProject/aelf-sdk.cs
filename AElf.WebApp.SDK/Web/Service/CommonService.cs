using System;
using System.Linq;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using AElf.WebApp.SDK.Web.Dto;
using Google.Protobuf;

namespace AElf.WebApp.SDK.Web.Service
{
    public interface INodeManager
    {
        Task<bool> IsConnected();
        Task<string> GetAccountFromPrivateKeyAsync(string privateKeyHex);
        Task<string> GetAccountFromPubKeyAsync(string pubKey);
        Task<string> GetPublicKey(string privateKeyHex);
        Task<string> GetGenesisContractAddressAsync();
        Task<Address> GetContractAddressByName(string contractName, string privateKeyHex);
    }

    public partial class AElfWebService : INodeManager
    {
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

        public async Task<string> GetGenesisContractAddressAsync()
        {
            var statusDto = await GetChainStatusAsync();
            var genesisAddress = statusDto.GenesisContractAddress;

            return genesisAddress;
        }

        public async Task<Address> GetContractAddressByName(string contractName, string privateKeyHex)
        {
            var hash = Hash.FromString(contractName);
            var from = await GetAccountFromPrivateKeyAsync(privateKeyHex);
            var to = await GetGenesisContractAddressAsync();
            var transaction = await GenerateTransaction(from, to, "GetContractAddressByName", hash);
            var txWithSig = await SignTransaction(privateKeyHex, transaction);

            var response = await ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSig.ToByteArray().ToHex()
            });
            var byteArray = ByteArrayHelper.HexStringToByteArray(response);
            var address = Address.Parser.ParseFrom(byteArray);

            return address;
        }

        #region private methods

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
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<Transaction> SignTransaction(string privateKeyHex, Transaction transaction)
        {
            var transactionData = transaction.GetHash().ToByteArray();

            // Sign the hash
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var signature = CryptoHelper.SignWithPrivateKey(privateKey, transactionData);
            transaction.Signature = ByteString.CopyFrom(signature);

            return Task.FromResult(transaction);
        }

        private ECKeyPair GetAElfKeyPair(string privateKeyHex)
        {
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyHex);
            var keyPair = CryptoHelper.FromPrivateKey(privateKey);

            return keyPair;
        }

        #endregion
    }
}