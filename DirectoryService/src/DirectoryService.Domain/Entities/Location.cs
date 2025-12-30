using CSharpFunctionalExtensions;
using SharedKernel;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class Location
{
    //ef core
    private Location() { }

    private readonly List<DepartmentLocation> _departments = [];

    private Location(
        Guid? id,
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone, 
        bool isActive,
        IEnumerable<DepartmentLocation> departments)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Address = address;
        Timezone = timezone;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        _departments = departments?.ToList() ?? new List<DepartmentLocation>();
    }

    public Guid Id { get; private set; }
    public LocationName Name { get; private set; } = null!;
    public LocationAddress Address { get; private set; } = null!;
    public LocationTimezone Timezone { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<DepartmentLocation> Departments => _departments;

    public static Result<Location, Errors> Create(
        Guid? id,
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone,
        bool isActive,
        IEnumerable<DepartmentLocation>? departments = null)
    {
        var location = new Location(id, name, address, timezone, isActive, departments!);

        return Result.Success<Location, Errors>(location);
    }
}

