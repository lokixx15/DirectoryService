namespace DirectoryService.Contracts;

public record PaginationRequest(
    int Page = 1,
    int Size = 20);
