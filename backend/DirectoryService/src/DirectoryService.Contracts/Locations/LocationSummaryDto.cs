namespace DirectoryService.Contracts.Locations;

public record LocationSummaryDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Timezone { get; init; } = string.Empty;

    public LocationAddressDto Address { get; set; } = null!;
}
