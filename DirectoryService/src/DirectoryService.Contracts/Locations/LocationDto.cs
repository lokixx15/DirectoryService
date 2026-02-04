namespace DirectoryService.Contracts.Locations;

public record LocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public LocationAddressDto Address { get; set; } = null!;
    public string Timezone { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
