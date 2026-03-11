using DomainLayer.Contracts;
using Her_Journey.CustomMiddleWares;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json;

namespace Her_Journey.Extensions
{
    public static class WebApplicationRegistration
    {
        public static async Task SeedDataBaseAsync(this WebApplication app)
        {
            var scoope = app.Services.CreateScope();
            var ObjectDataSeeding = scoope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await ObjectDataSeeding.IdentityDataSeedingAsync();


        }
        public static IApplicationBuilder UseCustomExcepationMiddleWare(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExpceptionHandlerMiddleWare>();
            return app;
        }
        public static IApplicationBuilder UseSwaggerMiddleWares(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(Options =>
            {
                Options.ConfigObject = new ConfigObject()
                {
                    DisplayRequestDuration = true
                };
                Options.DocumentTitle = "Her-Journey Api";
                Options.JsonSerializerOptions = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                Options.DocExpansion(DocExpansion.None);
                Options.EnableFilter();
                Options.EnablePersistAuthorization();
            });
            return app;
        }

    }
}
