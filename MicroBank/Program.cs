
using ApplicationLayer;
using InfrastructureLayer;

namespace MicroBank
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString =
                builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add services to the container.

            builder.Services
                .AddDataBase(connectionString)
                .AddMediatR_DI()
                .AddInfrastructure();


#region DefaultConfiguration
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
#endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
