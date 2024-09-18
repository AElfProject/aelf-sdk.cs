namespace AElf.Client.Dto
{
    public class SendMultiTransactionToGatewayInput
    {
        public string RawMultiTransaction { get; set; }
        public string GatewayUrl { get; set; }
    }
}