using System.Collections.Generic;

namespace AElf.Net.SDK.Infrastructure.Dto
{
    public class BlockBodyDto
    {
        public int TransactionsCount { get; set; }
        
        public List<string> Transactions { get; set; }
    }
}