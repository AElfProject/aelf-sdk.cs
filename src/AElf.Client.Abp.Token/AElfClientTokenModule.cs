using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Token;

[DependsOn(
    typeof(AElfClientModule)
)]
public class AElfClientTokenModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AElfTokenOptions>(options => { configuration.GetSection("AElfToken").Bind(options); });
    }
}