namespace DirectoryService.Contracts;

public record CursorPaginationResponse<T>(
    IReadOnlyList<T> Entities,
    string? NextCursor);