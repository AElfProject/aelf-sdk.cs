using System.Collections.Generic;

namespace AElf.Client.Dto
{
    public class SendUserSignedMultiTransactionOutput
    {
        public Dictionary<int, string[]> TxIdsDictionay { get; set; }
    }
}