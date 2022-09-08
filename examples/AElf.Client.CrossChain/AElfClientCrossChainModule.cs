using AElf.Client.Core;
using Volo.Abp.Modularity;

namespace AElf.Client.CrossChain;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientCrossChainModule : AbpModule
{
}