namespace AElf.Client.Infrastructure.Dto
{
    public class TransactionPoolStatusOutput
    {
        public int Queued { get; set; }
        public int Validated { get; set; }
    }
}