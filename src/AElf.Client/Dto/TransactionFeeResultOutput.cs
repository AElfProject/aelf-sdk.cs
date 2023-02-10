namespace AElf.Client.Dto;

public class TransactionFeeResultOutput
{

    public bool Success { get; set; }

    public Dictionary<string, long> TransactionFee { get; set; }

    public Dictionary<string, long> ResourceFee { get; set; }
}