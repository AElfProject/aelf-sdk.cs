using AElf.Client;
using AElf.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace TokenManager;

public class TokenManagerHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public TokenManagerHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<TokenManagerModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

         var tokenManagerService = _abpApplication.ServiceProvider.GetRequiredService<TokenManagerService>();
        // await tokenManagerService.TransferAsync(Address.FromBase58("eyDPrhJdofZ9f7Qdyi8FtbA8BePubP1M3gwfVMs6MnMHHNwik"),
        //     "ELF", 10000_00000000);
/*
        await tokenManagerService.CrossChainTransferAsync(
        Address.FromBase58("2HeW7S9HZrbRJZeivMppUuUY3djhWdfVnP5zrDsz8wqq6hKMfT"), "ELF", 100000_00000000,
        EndpointType.TestNetSideChain2.ToString());
        */
        await tokenManagerService.GetTokenInfoAsync("ELF");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}