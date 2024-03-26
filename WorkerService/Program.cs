using Microsoft.Azure.WebJobs;
using Showtime.Infrastructure;
using Showtime.WorkerService;

var builder = new HostBuilder();
builder.ConfigureWebJobs();
builder.ConfigureServices((context, services) =>
{
    services.AddInfrastructure(context.Configuration);
    services.AddTransient<IShowScraperService, ShowScraperService>();
});

var host = builder.Build();
var cancellationToken = new WebJobsShutdownWatcher().Token;

using (host)
{
    var jobHost = host.Services.GetService(typeof(IJobHost)) as JobHost;
    await host.StartAsync();

    await jobHost!.CallAsync("ExecuteAsync", new { cancellationToken }, cancellationToken);

    await host.StopAsync();
}