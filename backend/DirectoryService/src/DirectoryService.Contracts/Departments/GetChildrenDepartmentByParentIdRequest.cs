namespace DirectoryService.Contracts.Departments;

public record GetChildrenDepartmentByParentIdRequest(
    bool IsActiveOnly,
    int Page = 1,
    int Size = 20);
