using DirectoryService.Infrastructure.Seeding;
using Serilog;
using SharedService.Framework.Middlewares;

namespace DirectoryService.Presentation.Configuration;

public static class AppExtensions
{
    public static async Task<IApplicationBuilder> Configure(this WebApplication app, string[] args)
    {
        app.UseCors(builder =>
        {
            builder.WithOrigins("http://localhost:3000");
            builder.AllowCredentials();
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
        });

        app.UseSerilogRequestLogging();

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
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