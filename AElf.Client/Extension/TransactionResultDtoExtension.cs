using System.Collections.Generic;
using AElf.Client.Dto;
using AElf.Client.MultiToken;
using Google.Protobuf;

namespace AElf.Client.Extension
{
    public static class TransactionResultDtoExtension
    {
        public static Dictionary<string, long> GetTransactionFees(this TransactionResultDto transactionResultDto)
        {
            var transactionFeesDict = new Dictionary<string, long>();
            var eventLogs = transactionResultDto.Logs;
            if (eventLogs == null) return transactionFeesDict;
            foreach (var log in eventLogs)
            {
                if (log.Name == nameof(ResourceTokenCharged) || log.Name == nameof(TransactionFeeCharged))
                {
                    var info = TransactionFeeCharged.Parser.ParseFrom(ByteString.FromBase64(log.NonIndexed));
                    transactionFeesDict.Add(info.Symbol, info.Amount);
                }
            }

            return transactionFeesDict;
        }
    }
}