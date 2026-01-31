using DirectoryService.Application.Departments.Features.CreateDepartment;
using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests : DirectoryBaseTests
{
    public CreateDepartmentTests(DirectoryTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateDepartment_with_valid_data_should_succeed()
    {
        await SeedDataAsync();

        var locationId = await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations.FirstAsync();
            return location.Id;
        });

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Regional",
                "regional",
                null,
                [locationId]));

            return sut.Handle(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments.FirstAsync(d => d.Id == result.Value, cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id);
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_with_invalid_name_length_should_error()
    {
        await SeedDataAsync();

        var locationId = await ExecuteInDb(async dbContext =>
            await dbContext.Locations.Select(i => i.Id).FirstAsync());

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Re",
                "regional",
                null,
                [locationId]));

            return sut.Handle(command, cancellationToken);
        });

        Assert.NotNull(result.Error);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task CreateDepartment_with_multiple_locations_should_succeed()
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
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Regional",
                "regional",
                null,
                locationIds));

            return sut.Handle(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments.FirstAsync(d => d.Id == result.Value, cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id);
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartmentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();

        return await action(sut);
    }
}
