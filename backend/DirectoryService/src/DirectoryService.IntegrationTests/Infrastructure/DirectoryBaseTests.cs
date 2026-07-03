using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public abstract class DirectoryBaseTests : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    protected readonly DirectoryTestWebFactory Factory;

    protected DirectoryBaseTests(DirectoryTestWebFactory factory)
    {
        Factory = factory;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }

    protected async Task SeedDataAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();

        await seeder.SeedAsync(cancellationToken);
    }

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        return await action(sut);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await action(sut);
    }

    protected async Task<TResult> ExecuteHandler<THandler, TResult>(Func<THandler, Task<TResult>> action)
        where THandler : notnull
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<THandler>();

        return await action(sut);
    }
}
