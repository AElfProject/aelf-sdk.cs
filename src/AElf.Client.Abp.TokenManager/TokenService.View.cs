using AElf.Contracts.MultiToken;
using AElf.Types;
using Google.Protobuf;

namespace AElf.Client.Abp.TokenManager;

public partial class TokenService
{
    public async Task<TokenInfo> GetTokenInfoAsync(string symbol)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var result = await _clientService.ViewSystemAsync(TokenManagerConstants.TokenSmartContractName, "GetTokenInfo",
            new GetTokenInfoInput
            {
                Symbol = symbol
            }, useClientAlias);
        var tokenInfo = new TokenInfo();
        tokenInfo.MergeFrom(result);
        return tokenInfo;
    }

    public async Task<GetBalanceOutput> GetTokenBalanceAsync(string symbol, Address owner)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var result = await _clientService.ViewSystemAsync(TokenManagerConstants.TokenSmartContractName, "GetBalance",
            new GetBalanceInput
            {
                Owner = owner,
                Symbol = symbol
            }, useClientAlias);
        var balance = new GetBalanceOutput();
        balance.MergeFrom(result);
        return balance;
    }

    public async Task<GetAllowanceOutput> GetTokenAllowanceAsync(string symbol, Address owner, Address spender)
    {
        var useClientAlias = _clientConfigOptions.UseClientAlias;
        var result = await _clientService.ViewSystemAsync(TokenManagerConstants.TokenSmartContractName, "GetAllowance",
            new GetAllowanceInput
            {
                Owner = owner,
                Spender = spender,
                Symbol = symbol
            }, useClientAlias);
        var allowance = new GetAllowanceOutput();
        allowance.MergeFrom(result);
        return allowance;
    }
}