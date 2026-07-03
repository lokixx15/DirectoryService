using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Features.GetDepartments;
using DirectoryService.Application.Locations.Features.GetLocations;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.VO;
using DirectoryService.IntegrationTests.Infrastructure;
using SharedService.SharedKernel;

namespace DirectoryService.IntegrationTests.Departments;

public class GetDepartmentsTests : DirectoryBaseTests
{
    public GetDepartmentsTests(DirectoryTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetDepartments_with_valid_filters_should_succeed()
    {
        var cancellationToken = CancellationToken.None;

        var locationIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };
        var locations = new Location[] {
            Location.Create(
                locationIds[0], LocationName.Create("dacha").Value,
                LocationAddress.Create("Russia", "Novosibirsk", "Lenina", "b-25").Value,
                LocationTimezone.Create("Europe/Moscow").Value).Value,
            Location.Create(
                locationIds[1], LocationName.Create("dacha2").Value,
                LocationAddress.Create("Russia", "Novosibirsk", "Lenina", "b-21").Value,
                LocationTimezone.Create("Europe/Moscow").Value).Value,
        };
        await ExecuteInDb(async dbContext =>
        {
            await dbContext.Locations.AddRangeAsync(locations, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        });

        var parentDepartmentId = Guid.NewGuid();
        var departmentLocation = DepartmentLocation.Create(parentDepartmentId, locationIds[0]).Value;
        var parentDepartment = Department.CreateParent(
            parentDepartmentId,
            DepartmentName.Create("dep-parent").Value,
            DepartmentIdentifier.Create("depparenta").Value,
            [departmentLocation]).Value;

        await ExecuteInDb(async dbContext =>
        {
            await dbContext.Departments.AddAsync(parentDepartment, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        });

        var departmentIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var departments = new Department[4]
        {
            Department.CreateChild(departmentIds[0], DepartmentName.Create("dep-a").Value, DepartmentIdentifier.Create("depa").Value,
                parentDepartment, [DepartmentLocation.Create(departmentIds[0], locationIds[0]).Value]).Value,

            Department.CreateParent(departmentIds[1], DepartmentName.Create("dep-b").Value, DepartmentIdentifier.Create("depb").Value,
                [DepartmentLocation.Create(departmentIds[1], locationIds[0]).Value]).Value,

            Department.CreateChild(departmentIds[2], DepartmentName.Create("dep-c").Value, DepartmentIdentifier.Create("depc").Value,
                parentDepartment, [DepartmentLocation.Create(departmentIds[2], locationIds[1]).Value]).Value,

            Department.CreateChild(departmentIds[3], DepartmentName.Create("dep-d").Value, DepartmentIdentifier.Create("depd").Value,
                parentDepartment, [DepartmentLocation.Create(departmentIds[3], locationIds[0]).Value]).Value,
        };

        await ExecuteInDb(async dbContext =>
        {
            await dbContext.Departments.AddRangeAsync(departments, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        });

        var result = await ExecuteHandler<GetDepartmentsHandler, Result<PaginationResponse<DepartmentStandardDto>, Errors>>(sut =>
        {
            var query = new GetDepartmentsQuery(
                new GetDepartmentsRequest(
                    "dep",
                    parentDepartmentId,
                    [locationIds[0]],
                    [departmentIds[3]]));

            return sut.Handle(query, cancellationToken);
        });

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Entities);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal(1, result.Value.Entities?.Count);

        Assert.Equal("dep-a", result.Value.Entities?[0].Name);
        Assert.Equal("depa", result.Value.Entities?[0].Identifier);
        Assert.Equal("depparenta.depa", result.Value.Entities?[0].Path);
        Assert.Equal(true, result.Value.Entities?[0].IsActive);
    }
}
