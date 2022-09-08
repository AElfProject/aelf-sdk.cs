using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AElf.Client.TestBase;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule)
)]
public class AElfClientAbpTestBaseModule : AbpModule
{

}