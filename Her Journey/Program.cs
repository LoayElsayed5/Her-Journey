
using DomainLayer.Contracts;
using Her_Journey.Extensions;
using Persistence;
using Services.AuthServices;
using ServicesAbstraction.AuthServices;
using System.Threading.Tasks;

namespace Her_Journey
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddSwagerServices();
            builder.Services.AddJWTService(builder.Configuration);

            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            var app = builder.Build();

            var scoope = app.Services.CreateScope();
            var ObjectDataSeeding = scoope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await ObjectDataSeeding.IdentityDataSeedingAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
