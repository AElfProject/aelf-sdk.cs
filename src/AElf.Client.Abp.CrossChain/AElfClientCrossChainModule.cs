using Volo.Abp.Modularity;

namespace AElf.Client.Abp.CrossChain;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientCrossChainModule : AbpModule
{
}