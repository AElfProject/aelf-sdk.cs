using Google.Protobuf;

namespace AElf.Client.Abp;

public class ContractServiceBase
{
    private readonly IAElfClientService _clientService;
    private string SmartContractName { get; }

    protected ContractServiceBase(IAElfClientService clientService, string smartContractName)
    {
        _clientService = clientService;
        SmartContractName = smartContractName;
    }

    protected async Task<Transaction> PerformSendTransactionAsync(string methodName, IMessage parameter,
        string useClientAlias, string? smartContractName = null)
    {
        if (smartContractName == null)
        {
            smartContractName = SmartContractName;
        }

        return await _clientService.SendSystemAsync(smartContractName, methodName, parameter, useClientAlias);
    }

    protected async Task<TransactionResult> PerformGetTransactionResultAsync(string transactionId,
        string useClientAlias)
    {
        TransactionResult txResult;
        do
        {
            txResult = await _clientService.GetTransactionResultAsync(transactionId, useClientAlias);
        } while (txResult.Status == TransactionResultStatus.Pending);

        return txResult;
    }
}