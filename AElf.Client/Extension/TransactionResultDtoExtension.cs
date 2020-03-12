using System.Collections.Generic;
using AElf.Client.Dto;
using AElf.Client.MultiToken;
using Google.Protobuf;

namespace AElf.Client.Extension
{
    public static class TransactionResultDtoExtension
    {
        public static long GetDefaultTokenFee(this TransactionResultDto transactionResultDto)
        {
            var eventLogs = transactionResultDto.Logs;
            if (eventLogs == null)
            {
                return 0;
            }
            foreach (var log in eventLogs)
            {
                if (log.Name == nameof(TransactionFeeCharged))
                {
                    var info = TransactionFeeCharged.Parser.ParseFrom(ByteString.FromBase64(log.NonIndexed));
                    return info.Amount;
                }
            }

            return 0;
        }

        public static Dictionary<string, long> GetAllTokensFee(this TransactionResultDto transactionResultDto)
        {
            var dic = new Dictionary<string, long>();
            var eventLogs = transactionResultDto.Logs;
            if (eventLogs == null) return dic;
            foreach (var log in eventLogs)
            {
                if (log.Name == nameof(ResourceTokenCharged) || log.Name == nameof(TransactionFeeCharged))
                {
                    var info = TransactionFeeCharged.Parser.ParseFrom(ByteString.FromBase64(log.NonIndexed));
                    dic.Add(info.Symbol, info.Amount);
                }
            }

            return dic;
        }
    }
}