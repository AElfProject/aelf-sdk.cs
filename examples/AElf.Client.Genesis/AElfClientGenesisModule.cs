using AElf.Client.Consensus.AEDPoS;
using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Client.Parliament;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        Configure<AElfMinerAccountOptions>(options => { configuration.GetSection("AElfMinerAccount").Bind(options); });
    }
}