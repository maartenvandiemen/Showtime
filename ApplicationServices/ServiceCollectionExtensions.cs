using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Showtime.Core.Commands;

namespace Showtime.ApplicationServices
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // The list of open generic type we're interested in to register.
            var openGenericTypesToRegister = new[]
            {
                typeof(ICommandHandler<>)
            };

            var applicationAssembly = Assembly.GetExecutingAssembly();

            return services
                .Scan(scan => scan
                    .FromAssemblies(applicationAssembly)
                    .AddClasses(classes => classes.AssignableToAny(openGenericTypesToRegister))
                    .AsImplementedInterfaces(type => type.IsGenericType
                        && openGenericTypesToRegister.Contains(type.GetGenericTypeDefinition()))
                    .WithScopedLifetime());
        }
    }
}
