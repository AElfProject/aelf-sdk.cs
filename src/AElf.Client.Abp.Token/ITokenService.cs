using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;

namespace AElf.Client.Abp.Token;

public interface ITokenService
{
    Task<SendTransactionResult> CreateTokenAsync(Contracts.MultiToken.CreateInput createInput);
    Task<SendTransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput);
    Task<SendTransactionResult> MintNFTAsync(MintInput mintInput);
    Task<SendTransactionResult> ValidateTokenInfoExistsAsync(ValidateTokenInfoExistsInput validateTokenInfoExistsInput);
    Task<SendTransactionResult> CrossChainCreateTokenAsync(CrossChainCreateTokenInput crossChainCreateTokenInput);

    Task<TokenInfo> GetTokenInfoAsync(string symbol);
    Task<Contracts.MultiToken.GetBalanceOutput> GetTokenBalanceAsync(string symbol, Address owner);
    Task<Contracts.MultiToken.GetAllowanceOutput> GetTokenAllowanceAsync(string symbol, Address owner, Address spender);
}