namespace DirectoryService.Contracts.Locations;

public record CreateLocationRequest(
    string Name,
    string Timezone,
    CreateLocationAddressRequest Address);