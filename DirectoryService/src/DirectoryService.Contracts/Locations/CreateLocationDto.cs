namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(
    Guid? Id,
    string Name,
    string Address,
    string Timezone,
    bool IsActive);

//public static Result<Location> Create(
//        Guid? id,
//        LocationName name,
//        LocationAddress address,
//        LocationTimezone timezone,
//        bool isActive,
//        IEnumerable<DepartmentLocation> departments)
//    {
//        var location = new Location(id, name, address, timezone, isActive, departments);

//        return Result.Success(location);
//    }
