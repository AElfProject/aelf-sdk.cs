using AElf.Client.Abp;
using AElf.Client.Abp.CrossChain;
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