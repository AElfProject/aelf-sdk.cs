using AElf.Client.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp;

[DependsOn(
    typeof(AbpAutofacModule)
    )]
public class AElfClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AElfClientOptions>(options => { configuration.GetSection("AElfClient").Bind(options); });
        Configure<AElfAccountOptions>(options => { configuration.GetSection("AElfAccount").Bind(options); });
    }
}