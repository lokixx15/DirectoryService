namespace DirectoryService.Contracts.Positions;

public record GetPositionsRequest(
    string? Cursor,
    Guid[]? DepartmentIds,
    string? Search,
    bool IsActiveOnly,
    int PageSize = 20);
