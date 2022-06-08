using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;
using GetAllowanceOutput = AElf.Contracts.MultiToken.GetAllowanceOutput;
using GetBalanceOutput = AElf.Contracts.MultiToken.GetBalanceOutput;

namespace AElf.Client.Abp.TokenManager;

public interface ITokenService
{
    Task<TransactionResult> CreateTokenAsync(CreateInput createInput);
    Task<TransactionResult> CreateNFTProtocolAsync(Contracts.NFT.CreateInput createInput);
    Task<TransactionResult> MintNFTAsync(MintInput mintInput);
    Task<TransactionResult> SyncTokenInfoAsync(string symbol);

    Task<TokenInfo> GetTokenInfoAsync(string symbol);
    Task<GetBalanceOutput> GetTokenBalanceAsync(string symbol, Address owner);
    Task<GetAllowanceOutput> GetTokenAllowanceAsync(string symbol, Address owner, Address spender);
}