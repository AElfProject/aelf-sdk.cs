using AElf.Client.Abp.Genesis;
using AElf.Client.Abp.TestBase;
using AElf.Client.Abp.Token;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Test;

[DependsOn(
    typeof(AElfClientAbpTestBaseModule),
    typeof(AElfClientTokenModule),
    typeof(AElfClientGenesisModule)
)]
public class AElfClientAbpContractServiceTestModule : AbpModule
{
    
}