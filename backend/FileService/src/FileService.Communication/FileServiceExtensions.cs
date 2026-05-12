using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileService.Communication;

public static class FileServiceExtensions
{
    public static IServiceCollection AddFileHttpCommunication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FileServiceOptions>(configuration.GetSection(nameof(FileServiceOptions)));

        services.AddHttpClient<IFileService, FileHttpClient>((sp, config) =>
        {
            var fileOptions = sp.GetRequiredService<IOptions<FileServiceOptions>>().Value;

            config.BaseAddress = new Uri(fileOptions.Url);
            config.Timeout = TimeSpan.FromSeconds(fileOptions.TimeoutSeconds);
        });

        return services;
    }
}
