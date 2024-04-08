using AElf.Client.Abp;
using Volo.Abp.Modularity;

namespace AEDPoSViewer;

[DependsOn(
    typeof(AElfClientModule)
    )]
public class AEDPoSViewerModule : AbpModule
{

}