using Showtime.Infrastructure;
using Showtime.WorkerService;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration, true);

        services.AddHostedService<Worker>();
        services.AddTransient<IShowScraperService, ShowScraperService>();        
    })
    .Build();

host.Run();
