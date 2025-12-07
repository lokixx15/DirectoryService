using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class Location
{
    private readonly List<DepartmentLocation> _departments;

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
        _departments = departments.ToList();
    }

    public Guid Id { get; private set; }
    public LocationName Name { get; private set; }
    public LocationAddress Address { get; private set; }
    public LocationTimezone Timezone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<DepartmentLocation> Departments => _departments;

    public static Result<Location> Create(
        Guid? id,
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone,
        bool isActive,
        IEnumerable<DepartmentLocation> departments)
    {
        var location = new Location(id, name, address, timezone, isActive, departments);

        return Result.Success(location);
    }
}

