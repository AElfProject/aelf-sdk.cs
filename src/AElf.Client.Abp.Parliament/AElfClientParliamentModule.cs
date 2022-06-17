using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Parliament;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientParliamentModule : AbpModule
{
}