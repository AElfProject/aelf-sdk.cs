using AElf.Client.Abp;
using AElf.Client.Abp.Token;
using AElf.Client.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace TokenManager;

[DependsOn(
    typeof(AElfClientModule),
    typeof(AElfClientTokenModule)
)]
public class AEDPoSViewerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<TokenManagerOptions>(options => { configuration.GetSection("TokenManager").Bind(options); });
        Configure<SyncTokenInfoOptions>(options => { configuration.GetSection("SyncTokenInfo").Bind(options); });
    }
}