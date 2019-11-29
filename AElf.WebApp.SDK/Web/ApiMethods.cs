namespace AElf.WebApp.SDK.Web
{
    public enum ApiMethods
    {
        //Chain
        GetChainStatus,
        GetContractFileDescriptorSet,
        GetCurrentRoundInformation,
        GetRoundFromBase64,
        GetMiningSequences,

        //Block
        GetBlockHeight,
        GetBlockByHeight,
        GetBlockByHash,
        GetBlockState,

        //Transaction
        GetTaskQueueStatus,
        GetTransactionPoolStatus,
        ExecuteTransaction,
        ExecuteRawTransaction,
        CreateRawTransaction,
        SendRawTransaction,
        SendTransaction,
        SendTransactions,
        GetTransactionResult,
        GetTransactionResults,
        GetMerklePathByTransactionId,

        //Net
        GetPeers,
        AddPeer,
        RemovePeer,
        GetNetworkInfo
    }
}