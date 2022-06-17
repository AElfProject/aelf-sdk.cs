using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Consensus.AEDPoS;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientAEDPoSModule : AbpModule
{
}