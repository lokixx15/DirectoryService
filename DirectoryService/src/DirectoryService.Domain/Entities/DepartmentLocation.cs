using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Entities;

public class DepartmentLocation
{
    private DepartmentLocation(
        Guid departmentId, 
        Guid locationId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }

    public static Result<DepartmentLocation> Create(
        Guid departmentId,
        Guid locationId)
    {
        var departmentLocation = new DepartmentLocation(departmentId, locationId);

        return Result.Success(departmentLocation);
    }
}

