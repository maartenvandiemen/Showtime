using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Showtime.Infrastructure.Datastorage;
using Showtime.Infrastructure.ExternalRepository;

namespace Showtime.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services
            .AddDatabase(configuration)
            .AddExternalRepositories(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        var maxRetryCount = configuration
            .GetSection("Db")
            .GetValue<int>("MaxRetryCount");

        return services
            .AddDbContext<ShowDbContext>(options => options
                .UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount)))
            .AddScoped<DbContext>(sp => sp.GetRequiredService<ShowDbContext>())
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ShowDbContext>());
    }

    private static IServiceCollection AddExternalRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITvMazeRepository, TvMazeRepository>();
        services.AddScoped<ISyncStatusRepository, ShowtimeRepository>();
        services.AddScoped<IShowRepository, ShowtimeRepository>();

        var httpClient = services.AddHttpClient<ITvMazeRepository, TvMazeRepository>(client =>
        {
            client.BaseAddress = new Uri(configuration["baseUrlTvMazeApi"]!);
        });

        httpClient.AddRetryPolicy();

        return services;
    }

    private static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder httpClientBuilder)
    {
        var retryPolicy =
          Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryCount) =>
                {
                    return TimeSpan.FromSeconds(10 * retryCount);
                });

        return httpClientBuilder.AddPolicyHandler(retryPolicy);
    }
}
