using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Features.UpdateDepartmentParent;
using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentParentTests : DirectoryBaseTests
{
    public UpdateDepartmentParentTests(DirectoryTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task UpdateDepartmentParent_with_valid_data_should_succeed()
    {
        await SeedDataAsync();

        var department = await ExecuteInDb(async dbContext =>
            await dbContext.Departments
                .FirstAsync());

        var parentDepartment = await ExecuteInDb(async dbContext =>
            await dbContext.Departments
                .FirstAsync(d => d.Id != department.Id));

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<UpdateDepartmentParentHandler, UnitResult<Errors>>(sut =>
        {
            var command = new UpdateDepartmentParentCommand(
                department.Id,
                new UpdateDepartmentParentRequest(parentDepartment.Id));

            return sut.Handle(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentParent_with_null_parent_should_succeed()
    {
        await SeedDataAsync();

        var department = await ExecuteInDb(async dbContext =>
            await dbContext.Departments
                .FirstAsync());

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<UpdateDepartmentParentHandler, UnitResult<Errors>>(sut =>
        {
            var command = new UpdateDepartmentParentCommand(
                department.Id,
                new UpdateDepartmentParentRequest(null));

            return sut.Handle(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentParent_with_non_existance_parent_should_error()
    {
        await SeedDataAsync();

        var department = await ExecuteInDb(async dbContext =>
            await dbContext.Departments
                .FirstAsync());

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<UpdateDepartmentParentHandler, UnitResult<Errors>>(sut =>
        {
            var command = new UpdateDepartmentParentCommand(
                department.Id,
                new UpdateDepartmentParentRequest(Guid.NewGuid()));

            return sut.Handle(command, cancellationToken);
        });

        Assert.NotNull(result.Error);
        Assert.False(result.IsSuccess);
    }
}
