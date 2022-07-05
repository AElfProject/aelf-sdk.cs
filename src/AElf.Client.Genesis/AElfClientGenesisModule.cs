using AElf.Client.Consensus.AEDPoS;
using AElf.Client.Core;
using AElf.Client.Parliament;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace AElf.Client.Genesis;

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