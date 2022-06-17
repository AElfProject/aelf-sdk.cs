using AElf.Client.Abp.Consensus.AEDPoS;
using AElf.Client.Abp.Parliament;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Genesis;

[DependsOn(typeof(AElfClientModule),
    typeof(CoreAElfModule),
    typeof(AElfClientParliamentModule),
    typeof(AElfClientAEDPoSModule))]
public class AElfClientGenesisModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AElfContractOption>(configuration.GetSection("AElfContract"));
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
    }
}