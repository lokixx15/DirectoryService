using DirectoryService.Presentation.Middlewares;
using Serilog;

namespace DirectoryService.Presentation.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DirectoryService"));
        }

        app.MapControllers();

        return app;
    }
}