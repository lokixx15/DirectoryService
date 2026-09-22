namespace DirectoryService.Contracts.Positions;

public record GetPositionsRequest(
    string? Cursor,
    Guid[]? DepartmentIds,
    string? Search,
    int PageSize = 20);
