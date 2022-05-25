using AElf.Client;
using Volo.Abp.Modularity;

namespace AEDPoSViewer;

[DependsOn(
    typeof(AElfClientModule)
    )]
public class AEDPoSViewerModule : AbpModule
{

}