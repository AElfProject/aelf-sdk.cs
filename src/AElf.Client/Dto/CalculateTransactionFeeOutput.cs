using System.Collections.Generic;

namespace AElf.Client.Dto
{
    
    public class CalculateTransactionFeeOutput
    {
    
        public bool Success { get; set; }
    
        public Dictionary<string, long> TransactionFee { get; set; }
    
        public Dictionary<string, long> ResourceFee { get; set; }
    }}
