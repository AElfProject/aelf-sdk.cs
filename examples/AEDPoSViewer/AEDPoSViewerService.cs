using AEDPoSViewer.Data;
using AElf;
using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.Consensus.AEDPoS;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Spectre.Console;
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

    public async Task RunAsync()
    {
        var currentRound = await GetCurrentRoundAsync();
        for (var i = currentRound.RoundNumber - 4; i < currentRound.RoundNumber; i++)
        {
            DisplayRound(await GetRoundAsync(i));
        }

        DisplayRound(currentRound);
    }

    private async Task<Round> GetRoundAsync(long roundNumber)
    {
        using var scope = ServiceScopeFactory.CreateScope();

        var clientConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AElfClientConfigOptions>>();

        var clientService = scope.ServiceProvider.GetRequiredService<IAElfClientService>();

        var result = await clientService.ViewSystemAsync(AEDPoSViewerConstants.ConsensusSmartContractName,
            "GetRoundInformation", new Int64Value { Value = roundNumber }, clientConfig.Value.ClientAlias);

        var round = new Round();
        round.MergeFrom(result);
        return round;
    }

    private async Task<Round> GetCurrentRoundAsync()
    {
        using var scope = ServiceScopeFactory.CreateScope();

        var clientConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AElfClientConfigOptions>>();

        var clientService = scope.ServiceProvider.GetRequiredService<IAElfClientService>();

        var result = await clientService.ViewSystemAsync(AEDPoSViewerConstants.ConsensusSmartContractName,
            "GetCurrentRoundInformation", new Empty(), clientConfig.Value.ClientAlias);

        var round = new Round();
        round.MergeFrom(result);
        return round;
    }

    private void DisplayRound(Round round)
    {
        //AnsiConsole.MarkupLine($"[blue]Term:[/]\t{round.TermNumber}");
        AnsiConsole.MarkupLine($"[blue]Round:[/]\t{round.RoundNumber}");
        //AnsiConsole.MarkupLine($"[blue]Days:[/]\t{new TimeSpan(0, 0, 0, (int)round.BlockchainAge).TotalDays}");

        var table = new Table();
        table.AddColumn(new TableColumn(new Markup("[blue]Pubkey[/]")));
        table.AddColumn(new TableColumn("[blue]Address[/]"));
        table.AddColumn(new TableColumn("[blue]Order[/]"));
        table.AddColumn(new TableColumn("[blue]Produced[/]"));
        table.AddColumn(new TableColumn("[blue]Missed Slots[/]"));
        foreach (var info in round.RealTimeMinersInformation.OrderBy(i => i.Value.Order))
        {
            var pubkey = info.Key;
            var minerInRound = info.Value;
            var address = Address.FromPublicKey(ByteArrayHelper.HexStringToByteArray(pubkey)).ToBase58();
            BlockProducer.Map.TryGetValue(pubkey, out var displayPubkey);
            var actualMiningTimes = minerInRound.ActualMiningTimes.Count;
            var isAlarm = actualMiningTimes == 0;
            table.AddRow(
                (isAlarm ? "[red]" : "") + (displayPubkey ?? pubkey[..10]) + (isAlarm ? "[/]" : ""),
                address,
                minerInRound.Order.ToString(),
                (isAlarm ? "[red]" : "") + actualMiningTimes + (isAlarm ? "[/]" : ""),
                minerInRound.MissedTimeSlots.ToString());
        }

        AnsiConsole.Write(table);
    }
}