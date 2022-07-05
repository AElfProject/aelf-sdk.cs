using AElf.Client.Core;
using AElf.Contracts.Whitelist;

namespace AElf.Client.Whitelist;

public interface IWhitelistService
{
    Task<SendTransactionResult> CreateWhitelistAsync(CreateWhitelistInput createWhitelistInput);
}