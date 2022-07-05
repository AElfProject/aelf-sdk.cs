using AElf.Client.Core;
using AElf.Client.CrossChain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace TokenManager;

[DependsOn(
    typeof(AElfClientModule),
    typeof(AElfClientCrossChainModule)
)]
public class CrossChainManagerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
    }
}