using AElf.Client.Core;
using AElf.Client.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace TokenManager;

[DependsOn(
    typeof(AElfClientModule),
    typeof(AElfClientTokenModule)
)]
public class TokenManagerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<TokenManagerOptions>(options => { configuration.GetSection("TokenManager").Bind(options); });
    }
}