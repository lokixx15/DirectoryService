using DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;
using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentLocationsTests : DirectoryBaseTests
{
    public UpdateDepartmentLocationsTests(DirectoryTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task UpdateDepartmentLocations_with_valid_data_should_succeed()
    {
        await SeedDataAsync();

        var locationIds = await ExecuteInDb(async dbContext =>
            await dbContext.Locations
                .Where(l => l.Timezone.Value == "Europe/Moscow")
                .Select(l => l.Id)
                .ToArrayAsync());

        var department = await ExecuteInDb(async dbContext =>
            await dbContext.Departments
                .FirstAsync());

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                department.Id,
                new UpdateDepartmentLocationsRequest(locationIds));

            return sut.Handle(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentLocations_with_nonexistent_department_should_error()
    {
        await SeedDataAsync();

        var locationIds = await ExecuteInDb(async dbContext =>
            await dbContext.Locations
                .Where(l => l.Timezone.Value == "Europe/Moscow")
                .Select(l => l.Id)
                .ToArrayAsync());

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                Guid.NewGuid(),
                new UpdateDepartmentLocationsRequest(locationIds));

            return sut.Handle(command, cancellationToken);
        });

        Assert.NotNull(result.Error);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentLocations_with_nonexistent_locations_should_error()
    {
        await SeedDataAsync();

        var department = await ExecuteInDb(async dbContext =>
            await dbContext.Departments
                .FirstAsync());

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                department.Id,
                new UpdateDepartmentLocationsRequest([Guid.NewGuid(), Guid.NewGuid()]));

            return sut.Handle(command, cancellationToken);
        });

        Assert.NotNull(result.Error);
        Assert.False(result.IsSuccess);
    }

    private async Task<T> ExecuteHandler<T>(Func<UpdateDepartmentLocationsHadnler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<UpdateDepartmentLocationsHadnler>();

        return await action(sut);
    }
}