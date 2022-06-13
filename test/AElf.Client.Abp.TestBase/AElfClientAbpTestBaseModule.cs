using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.TestBase;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule)
)]
public class AElfClientAbpTestBaseModule : AbpModule
{

}