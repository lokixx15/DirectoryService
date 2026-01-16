namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(
    string Name,
    string Timezone,
    CreateLocationAddressDto Address);