using AElf.Client.Core;
using Volo.Abp.Modularity;

namespace AElf.Client.Parliament;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientParliamentModule : AbpModule
{
}