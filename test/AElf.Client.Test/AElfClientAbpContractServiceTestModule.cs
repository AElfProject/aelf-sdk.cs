using AElf.Client.Genesis;
using AElf.Client.TestBase;
using AElf.Client.Token;
using Volo.Abp.Modularity;

namespace AElf.Client.Test;

[DependsOn(
    typeof(AElfClientAbpTestBaseModule),
    typeof(AElfClientTokenModule),
    typeof(AElfClientGenesisModule)
)]
public class AElfClientAbpContractServiceTestModule : AbpModule
{
    
}