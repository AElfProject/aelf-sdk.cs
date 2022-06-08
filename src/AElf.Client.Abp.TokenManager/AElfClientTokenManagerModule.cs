using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.TokenManager;

[DependsOn(
    typeof(AElfClientModule)
)]
public class AElfClientTokenManagerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<TokenManagerOptions>(options => { configuration.GetSection("TokenManager").Bind(options); });
    }
}