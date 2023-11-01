using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WorldDiabetesFoundation.Core.Infrastructure.Integration.Interfaces;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Core.Infrastructure.AutoRegister
{
    public static class HttpClientRegistration
    {
        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            // Using reflection to get all types which implement IHttpClient and are not abstract.
            var httpClient = typeof(IHttpClient);
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes()  // You can use Assembly.GetAssembly(typeof(ProjectFetcher)) if your services are located in a different assembly.
                .Where(t => !t.IsInterface && !t.IsAbstract && httpClient.IsAssignableFrom(t));

            foreach (var type in serviceTypes)
            {
                // Register a named HttpClient for the type.
                services.AddHttpClient(type.FullName);
                
                // Register the service type with its interface.
                services.AddTransient(type);
            }
            
            return services;
        }
    }
}