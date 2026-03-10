using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations.VO;
using SharedService.SharedKernel;

namespace DirectoryService.Domain.Locations;

public class Location
{
    // ef core
    private Location() { }

    private readonly List<DepartmentLocation> _departments = [];

    private Location(
        Guid? id,
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone,
        IEnumerable<DepartmentLocation> departments)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Address = address;
        Timezone = timezone;
        IsActive = true;
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

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> Departments => _departments;

    public static Result<Location, Errors> Create(
        Guid? id,
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone,
        IEnumerable<DepartmentLocation>? departments = null)
    {
        var location = new Location(id, name, address, timezone, departments!);

        return Result.Success<Location, Errors>(location);
    }
}