using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.DepartmentLocations;

public class DepartmentLocation
{
    //ef core
    private DepartmentLocation() { }

    private DepartmentLocation(
        Guid departmentId, 
        Guid locationId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid Id { get; }
    public Guid DepartmentId { get; }
    public Guid LocationId { get; }

    public static Result<DepartmentLocation, Errors> Create(
        Guid departmentId,
        Guid locationId)
    {
        var departmentLocation = new DepartmentLocation(departmentId, locationId);

        return Result.Success<DepartmentLocation, Errors>(departmentLocation);
    }
}

