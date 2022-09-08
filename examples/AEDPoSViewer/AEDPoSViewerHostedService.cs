using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace AEDPoSViewer;

public class AEDPoSViewerHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public AEDPoSViewerHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<AEDPoSViewerModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

        var viewerService = _abpApplication.ServiceProvider.GetRequiredService<AEDPoSViewerService>();
        await viewerService.RunAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}