namespace DirectoryService.Contracts;

public record PaginationResponse<T>(
    IReadOnlyList<T> Entities,
    long TotalCount);