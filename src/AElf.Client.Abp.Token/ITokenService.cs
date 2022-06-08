using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;

namespace AElf.Client.Abp.Token;

public interface ITokenService
{
    Task<TransactionResult> CreateTokenAsync(Contracts.MultiToken.CreateInput createInput);
    Task<TransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput);
    Task<TransactionResult> MintNFTAsync(MintInput mintInput);
    Task<TransactionResult> SyncTokenInfoAsync(string symbol);

    Task<TokenInfo> GetTokenInfoAsync(string symbol);
    Task<Contracts.MultiToken.GetBalanceOutput> GetTokenBalanceAsync(string symbol, Address owner);
    Task<Contracts.MultiToken.GetAllowanceOutput> GetTokenAllowanceAsync(string symbol, Address owner, Address spender);
}