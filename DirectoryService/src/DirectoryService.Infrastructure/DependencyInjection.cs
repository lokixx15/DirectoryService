using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using DirectoryService.Infrastructure.BackgroundTasks.ClearingInactiveDepartments;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Departments;
using DirectoryService.Infrastructure.Locations;
using DirectoryService.Infrastructure.Positions;
using DirectoryService.Infrastructure.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<ClearingInactiveDepartmentsOptions>(
                configuration.GetSection("ClearingInactiveDepartmentsOptions"))
            .AddHostedService<ClearingInactiveDepartmentsService>()
            .AddScoped(_ => new DirectoryServiceDbContext(
                configuration.GetConnectionString("DirectoryServiceDb")!))
            .AddScoped<ISeeder, DirectoryServiceSeeder>()
            .AddScoped<IDbConnectionFactory, DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(configuration.GetConnectionString("DirectoryServiceDb")!))
            .AddScoped<ITransactionManager, TransactionManager>()
            .AddScoped<ILocationsRepository, LocationsRepository>()
            .AddScoped<IDepartmentsRepository, DepartmentsRepository>()
            .AddScoped<IPositionsRepository, PositionsRepository>();
}