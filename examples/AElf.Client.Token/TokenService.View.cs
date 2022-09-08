using AElf.Contracts.MultiToken;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace AElf.Client.Token;

public partial class TokenService
{
    public async Task<TokenInfo> GetTokenInfoAsync(string symbol)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<TokenInfo>(AElfTokenConstants.TokenSmartContractName,
            "GetTokenInfo", new GetTokenInfoInput
            {
                Symbol = symbol
            }, useClientAlias);
    }

    public async Task<GetBalanceOutput> GetTokenBalanceAsync(string symbol, Address owner)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<GetBalanceOutput>(AElfTokenConstants.TokenSmartContractName,
            "GetBalance", new GetBalanceInput
            {
                Owner = owner,
                Symbol = symbol
            }, useClientAlias);
    }

    public async Task<GetAllowanceOutput> GetTokenAllowanceAsync(string symbol, Address owner, Address spender)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<GetAllowanceOutput>(AElfTokenConstants.TokenSmartContractName,
            "GetAllowance", new GetAllowanceInput
            {
                Owner = owner,
                Spender = spender,
                Symbol = symbol
            }, useClientAlias);
    }

    public async Task<CalculateFeeCoefficients> GetCalculateFeeCoefficientsForSenderAsync()
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        return await _clientService.ViewSystemAsync<CalculateFeeCoefficients>(AElfTokenConstants.TokenSmartContractName,
            "GetCalculateFeeCoefficientsForSender", new Empty(), useClientAlias);
    }
}