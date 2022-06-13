using AElf.Types;

namespace AElf.Client.Abp.Token.CrossChainTransfer;

public interface ICrossChainTransferService
{
    Task CrossChainTransferAsync(Address to, string symbol, long amount, string fromClientAlias, string toClientAlias);
}