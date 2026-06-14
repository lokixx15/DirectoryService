namespace DirectoryService.Contracts.Positions;

public record PositionDto(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DeletedAt);