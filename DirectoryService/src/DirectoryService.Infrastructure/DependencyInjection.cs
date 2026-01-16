using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Departments;
using DirectoryService.Infrastructure.Locations;
using DirectoryService.Infrastructure.Positions;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services
            .AddScoped<DirectoryServiceDbContext>()
            .AddScoped<ITransactionManager, TransactionManager>()
            .AddScoped<ILocationsRepository, LocationsRepository>()
            .AddScoped<IDepartmentsRepository, DepartmentsRepository>()
            .AddScoped<IPositionsRepository, PositionsRepository>();
}