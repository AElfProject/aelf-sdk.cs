using System.Collections.Generic;
using System.Linq;
using AElf.Client.Dto;
using AElf.Client.MultiToken;
using Google.Protobuf;

namespace AElf.Client.Extension
{
    public static class TransactionResultDtoExtension
    {
        public static Dictionary<string, long> GetTransactionFees(this TransactionResultDto transactionResultDto)
        {
            var relatedLogs = transactionResultDto.Logs?.Where(l => l.Name == nameof(TransactionFeeCharged)).ToList();
            if (relatedLogs == null || !relatedLogs.Any())
            {
                return new Dictionary<string, long>();
            }

            return relatedLogs.Select(l => TransactionFeeCharged.Parser.ParseFrom(ByteString.FromBase64(l.NonIndexed)))
                .ToDictionary(e => e.Symbol, e => e.Amount);
        }
    }
}