using System;
using AElf.Client.Dto;

namespace AElf.Client.Test
{
    public static class Extensions
    {
        public static bool IsComplete(this BlockDto block)
        {
            if (block == null
                || block.BlockHash.IsNullOrWhiteSpace()
                || block.Header == null
                || block.Header.Bloom.IsNullOrWhiteSpace()
                || block.Header.Extra.IsNullOrWhiteSpace()
                || block.Header.Height <= 0
                || block.Header.SignerPubkey.IsNullOrWhiteSpace()
                || block.Header.ChainId.IsNullOrWhiteSpace()
                || block.Header.PreviousBlockHash.IsNullOrWhiteSpace()
                || block.Header.MerkleTreeRootOfTransactions.IsNullOrWhiteSpace()
                || block.Header.MerkleTreeRootOfTransactionState.IsNullOrWhiteSpace()
                || block.Header.MerkleTreeRootOfWorldState.IsNullOrWhiteSpace()
                || block.Body == null
                || block.Body.TransactionsCount == 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsCompleteWithTransaction(this BlockDto block)
        {
            if (!block.IsComplete()
                || block.Body.Transactions == null
                || block.Body.Transactions.Count != block.Body.TransactionsCount)
            {
                return false;
            }

            return true;
        }
    }
}