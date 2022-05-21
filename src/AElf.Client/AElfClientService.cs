using AElf.Client.Options;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client;

public class AElfClientService : IAElfClientService, ITransientDependency
{
    private readonly IAElfClientProvider _aelfClientProvider;
    private readonly AElfClientOptions _aelfClientOptions;
    private readonly AElfAccountOptions _aelfAccountOptions;

    public AElfClientService(IAElfClientProvider aelfClientProvider, IOptionsSnapshot<AElfClientOptions> aelfClientOptions,
        IOptionsSnapshot<AElfAccountOptions> aelfAccountOptions)
    {
        _aelfClientProvider = aelfClientProvider;
        _aelfClientOptions = aelfClientOptions.Value;
        _aelfAccountOptions = aelfAccountOptions.Value;
        
    }
}