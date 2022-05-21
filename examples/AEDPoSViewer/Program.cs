using AEDPoSViewer;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
    .MinimumLevel.Override("AEDPoSViewer", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("AEDPoSViewer", LogEventLevel.Information)
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File($"Logs/aelf-consensus-viewer-{DateTime.UtcNow:yyyy-MM-dd}.logs"))
    .WriteTo.Console()
    .CreateLogger();

using var application = AbpApplicationFactory.Create<AEDPoSViewerModule>(
    options =>
    {
        options.UseAutofac();
        options.Services.AddLogging(c => c.AddSerilog());
    });
application.Initialize();

await application.ServiceProvider
    .GetRequiredService<AEDPoSViewerService>()
    .RunAsync(args);

application.Shutdown();
Log.CloseAndFlush();