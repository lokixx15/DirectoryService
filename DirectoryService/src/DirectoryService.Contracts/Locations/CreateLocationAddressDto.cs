namespace DirectoryService.Contracts.Locations;

public record CreateLocationAddressDto(
    string Country,
    string City,
    string Street,
    string Building,
    string? Region = null,
    string? District = null,
    string? Apartment = null);