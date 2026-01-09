namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(
    string Name,
    string Timezone,
    bool IsActive,
    CreateLocationAddressDto Address);
