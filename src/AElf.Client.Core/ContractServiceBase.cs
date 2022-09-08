using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Threading;

namespace AElf.Client.Core;

public class ContractServiceBase
{
    private readonly IAElfClientService _clientService;
    protected string? SmartContractName { get; }
    protected Address? ContractAddress { get; set; }

    public ILogger<ContractServiceBase> Logger { get; set; }

    protected ContractServiceBase(IAElfClientService clientService, string smartContractName)
    {
        _clientService = clientService;
        SmartContractName = smartContractName;

        Logger = NullLogger<ContractServiceBase>.Instance;
    }

    protected ContractServiceBase(IAElfClientService clientService, Address contractAddress)
    {
        _clientService = clientService;
        ContractAddress = contractAddress;

        Logger = NullLogger<ContractServiceBase>.Instance;
    }

    protected async Task<Transaction?> PerformSendTransactionAsync(string methodName, IMessage parameter,
        string clientAlias)
    {
        if (ContractAddress != null)
        {
            return await _clientService.SendAsync(ContractAddress.ToBase58(), methodName, parameter, clientAlias);
        }

        if (SmartContractName != null)
        {
            return await _clientService.SendSystemAsync(SmartContractName, methodName, parameter, clientAlias);
        }

        Logger.LogError($"Neither ContractAddress nor SmartContractName is set.");
        return null;
    }

    protected async Task<TransactionResult> PerformGetTransactionResultAsync(string transactionId,
        string clientAlias)
    {
        TransactionResult txResult;
        do
        {
            txResult = await _clientService.GetTransactionResultAsync(transactionId, clientAlias);
        } while (txResult.Status == TransactionResultStatus.Pending);

        Logger.LogInformation("{TxResult}", txResult);
        return txResult;
    }
}