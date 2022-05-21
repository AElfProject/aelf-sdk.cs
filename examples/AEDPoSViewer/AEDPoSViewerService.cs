using AElf.Client;
using AElf.Contracts.Consensus.AEDPoS;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace AEDPoSViewer;

public class AEDPoSViewerService : ITransientDependency
{
    private IServiceScopeFactory ServiceScopeFactory { get; }

    public ILogger<AEDPoSViewerService> Logger { get; set; }

    public AEDPoSViewerService(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;

        Logger = NullLogger<AEDPoSViewerService>.Instance;
    }

    public async Task RunAsync(string[] args)
    {
        using var scope = ServiceScopeFactory.CreateScope();

        var clientService = scope.ServiceProvider.GetRequiredService<IAElfClientService>();

        var result = await clientService.ViewSystemAsync(AEDPoSViewerConstants.ConsensusSmartContractName,
            "GetCurrentRoundInformation", new Empty(), EndpointType.MainNetMainChain.ToString());

        var round = new Round();
        round.MergeFrom(result);
        Logger.LogInformation(round.ToString());
    }
}