
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



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddSwagerServices();
            builder.Services.AddJWTService(builder.Configuration);

            builder.Services.AddWebApplicationServices();


            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            var app = builder.Build();

           await app.SeedDataBaseAsync();

            app.UseCustomExcepationMiddleWare();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleWares();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
