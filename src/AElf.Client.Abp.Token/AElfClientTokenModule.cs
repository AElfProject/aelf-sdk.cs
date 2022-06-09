﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Token;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientTokenModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AElfTokenOptions>(options => { configuration.GetSection("AElfToken").Bind(options); });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var taskQueueManager = context.ServiceProvider.GetService<ITaskQueueManager>();
        taskQueueManager?.CreateQueue(AElfTokenConstants.SyncTokenInfoQueueName,
            AElfTokenConstants.SyncTokenInfoMaxDegreeOfParallelism);
    }
}