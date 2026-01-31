using DirectoryService.Infrastructure.Seeding;
using DirectoryService.Presentation.Middlewares;
using Serilog;

namespace DirectoryService.Presentation.Configuration;

public static class AppExtensions
{
    public static async Task<IApplicationBuilder> Configure(this WebApplication app, string[] args)
    {
        app.UseSerilogRequestLogging();

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DirectoryService"));

            if (args.Contains("--seeding"))
            {
                await app.Services.RunSeeding();
            }
        }

        app.MapControllers();

        return app;
    }
}