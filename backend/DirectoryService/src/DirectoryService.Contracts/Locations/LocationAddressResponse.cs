namespace DirectoryService.Contracts.Locations;

public record LocationAddressDto
{
    public string Country { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string Street { get; init; } = string.Empty;

    public string Building { get; init; } = string.Empty;

    public string? Region { get; init; } = null!;

    public string? District { get; init; } = null!;

    public string? Apartment { get; init; } = null!;
}