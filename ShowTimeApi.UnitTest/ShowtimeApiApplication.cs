using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Showtime.Infrastructure.Datastorage;

namespace Showtime.ShowtimeApi.UnitTests;
internal class ShowtimeApiApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            //First remove the serviceDescriptor added in the Program.cs if available
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ShowDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            //Add a new InMemoryDatabase each time this class is initialized.
            //We give the database each time a new name in order to ensure that tests can be executed in parallel.
            //Lifetime is singleton within this class. Since some tests might create multiple Scopes the Singleton is the safest.
            services.AddDbContext<ShowDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
        });
    }
}
