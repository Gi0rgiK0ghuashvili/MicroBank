using ApplicationLayer.Interfaces;
using ApplicationLayer.Persistance;
using DomainLayer;
using InfrastructureLayer.Repositories;
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
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped(typeof(Result< >));

            return services;
        }

        public static IServiceCollection AddDataBase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>((options) =>
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
