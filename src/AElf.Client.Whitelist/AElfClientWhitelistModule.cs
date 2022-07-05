using AElf.Client.Core;
using Volo.Abp.Modularity;

namespace AElf.Client.Whitelist;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientWhitelistModule
{
    
}