using AElf.Contracts.Whitelist;

namespace AElf.Client.Abp.Whitelist;

public interface IWhitelistService
{
    Task<SendTransactionResult> CreateWhitelistAsync(CreateWhitelistInput createWhitelistInput);
}