using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Entities;

public class DepartmentPosition
{
    //ef core
    private DepartmentPosition() { }
    private DepartmentPosition(
    Guid departmentId,
    Guid positionId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid PositionId { get; private set; }

    public static Result<DepartmentPosition> Create(
        Guid departmentId,
        Guid positionId)
    {
        var departmentPosition = new DepartmentPosition(departmentId, positionId);

        return Result.Success(departmentPosition);
    }
}

