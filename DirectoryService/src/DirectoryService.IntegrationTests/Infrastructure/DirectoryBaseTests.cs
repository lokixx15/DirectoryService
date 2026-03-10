using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryBaseTests : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    protected readonly IServiceProvider Services;
    private readonly Func<Task> _resetDatabase;

    public DirectoryBaseTests(DirectoryTestWebFactory factory)
    {
        Services = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    protected async Task SeedDataAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();

        await seeder.SeedAsync(cancellationToken);
    }

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        return await action(sut);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await action(sut);
    }
}