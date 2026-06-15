namespace DirectoryService.Contracts.Positions;

public record GetPositionsRequest(
    string? Cursor,
    Guid[]? DepartmentIds,
    string? Search,
    bool? IsActive,
    int PageSize = 20);
