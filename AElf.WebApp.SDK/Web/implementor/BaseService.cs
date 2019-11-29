using System.Collections.Generic;

namespace AElf.WebApp.SDK.Web.Service
{
    public class BaseService
    {
        protected BaseService()
        {
            InitializeWebApiRoute();
        }
        
        private Dictionary<ApiMethods, string> _apiRoute;
        protected string BaseUrl { get; set; }

        protected string FormatServiceUrl(string serviceUrl)
        {
            if (serviceUrl.Contains("http://") || serviceUrl.Contains("https://"))
                return serviceUrl;

            return $"http://{serviceUrl}";
        }

        protected string GetRequestUrl(ApiMethods api, params object[] parameters)
        {
            var subUrl = string.Format(_apiRoute[api], parameters);

            return $"{BaseUrl}{subUrl}";
        }

        private void InitializeWebApiRoute()
        {
            _apiRoute = new Dictionary<ApiMethods, string>
            {
                //chain route
                {ApiMethods.GetChainStatus, "/api/blockChain/chainStatus"},
                {ApiMethods.GetContractFileDescriptorSet, "/api/blockChain/contractFileDescriptorSet?address={0}"},
                {
                    ApiMethods.GetCurrentRoundInformation, "/api/blockChain/currentRoundInformation"
                },

                {ApiMethods.GetRoundFromBase64, "/api/blockChain/roundFromBase64?str={0}"},
                {ApiMethods.GetMiningSequences, "/api/blockChain/miningSequences?count={0}"},

                //transaction route
                {ApiMethods.CreateRawTransaction, "/api/blockChain/rawTransaction"},
                {ApiMethods.GetTransactionPoolStatus, "/api/blockChain/transactionPoolStatus"},
                {ApiMethods.SendTransaction, "/api/blockChain/sendTransaction"},
                {ApiMethods.SendTransactions, "/api/blockChain/sendTransactions"},
                {ApiMethods.SendRawTransaction, "/api/blockChain/sendRawTransaction"},
                {ApiMethods.ExecuteTransaction, "/api/blockChain/executeTransaction"},
                {ApiMethods.ExecuteRawTransaction, "/api/blockChain/executeRawTransaction"},
                {ApiMethods.GetTransactionResult, "/api/blockChain/transactionResult?transactionId={0}"},
                {
                    ApiMethods.GetTransactionResults,
                    "/api/blockChain/transactionResults?blockHash={0}&offset={1}&limit={2}"
                },
                {
                    ApiMethods.GetMerklePathByTransactionId,
                    "/api/blockChain/merklePathByTransactionId?transactionId={0}"
                },
                {ApiMethods.GetTaskQueueStatus, "/api/blockChain/taskQueueStatus"},

                //block route
                {ApiMethods.GetBlockHeight, "/api/blockChain/blockHeight"},
                {ApiMethods.GetBlockState, "/api/blockChain/blockState?blockHash={0}"},
                {ApiMethods.GetBlockByHeight, "/api/blockChain/blockByHeight?blockHeight={0}&includeTransactions={1}"},
                {ApiMethods.GetBlockByHash, "/api/blockChain/block?blockHash={0}&includeTransactions={1}"},

                //net route
                {ApiMethods.GetPeers, "/api/net/peers?withMetrics={0}"},
                {ApiMethods.AddPeer, "/api/net/peer"},
                {ApiMethods.RemovePeer, "/api/net/peer?address={0}"},
                {ApiMethods.GetNetworkInfo, "/api/net/networkInfo"}
            };
        }
    }
}