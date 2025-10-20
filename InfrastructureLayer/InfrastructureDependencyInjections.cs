using ApplicationLayer.Interfaces;
using ApplicationLayer.Persistance;
using DomainLayer;
using InfrastructureLayer.Interfaces;
using InfrastructureLayer.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureLayer
{
    public static class InfrastructureDependencyInjections
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(
                typeof(IGenericRepository< >),
                typeof(GenericRepository< >));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHandler, PasswordHandler>();
            services.AddScoped(typeof(Result< >));

            return services;
        }

        public static IServiceCollection AddDataBase(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>()
                .AddDbContext<ApplicationDbContext>((options) =>
                {
                    options.UseSqlServer(connectionString);
#if DEBUG
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
#endif
                });

            return services;
        }

    }
}
