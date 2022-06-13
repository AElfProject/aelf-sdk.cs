using AElf.Client.Abp.TestBase;
using AElf.Client.Abp.Token;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Test;

[DependsOn(
    typeof(AElfClientAbpTestBaseModule),
    typeof(AElfClientTokenModule)
    )]
public class AElfClientAbpContractServiceTestModule : AbpModule
{
    
}