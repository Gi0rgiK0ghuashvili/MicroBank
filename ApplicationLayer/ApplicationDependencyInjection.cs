using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApplicationLayer
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddMediatR_DI(this IServiceCollection services) 
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
