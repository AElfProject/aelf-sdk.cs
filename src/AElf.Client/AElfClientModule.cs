using AElf.Client.Options;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AElf.Client;

[DependsOn(
    typeof(AbpAutofacModule)
    )]
public class AElfClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AElfClientOptions>(context.Services.GetConfiguration().GetSection("AElfClient"));
    }
}