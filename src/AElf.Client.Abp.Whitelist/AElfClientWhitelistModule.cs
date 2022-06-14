using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Whitelist;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientWhitelistModule
{
    
}