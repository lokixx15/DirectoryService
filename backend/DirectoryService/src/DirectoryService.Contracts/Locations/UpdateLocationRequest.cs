namespace DirectoryService.Contracts.Locations;

public record UpdateLocationRequest(
    string Name,
    string Timezone,
    CreateLocationAddressRequest Address);