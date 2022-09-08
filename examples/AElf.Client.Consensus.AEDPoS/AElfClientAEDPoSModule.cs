using AElf.Client.Core;
using Volo.Abp.Modularity;

namespace AElf.Client.Consensus.AEDPoS;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientAEDPoSModule : AbpModule
{
}