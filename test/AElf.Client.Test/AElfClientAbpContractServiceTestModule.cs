using AElf.Client.TestBase;
using AElf.Client.Token;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Test;

[DependsOn(
    typeof(AElfClientAbpTestBaseModule),
    typeof(AElfClientTokenModule)
    )]
public class AElfClientAbpContractServiceTestModule : AbpModule
{
    
}