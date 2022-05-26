namespace AElf.Client.Dto;

public class ExecuteRawTransactionDto
{
    /// <summary>
    /// raw transaction
    /// </summary>
    public string RawTransaction { get; set; }

    /// <summary>
    /// signature
    /// </summary>
    public string Signature { get; set; }
}