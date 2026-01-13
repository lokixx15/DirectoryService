using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.DepartmentPositions;

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

    public Guid Id { get; }
    public Guid DepartmentId { get; }
    public Guid PositionId { get; }

    public static Result<DepartmentPosition, Errors> Create(
        Guid departmentId,
        Guid positionId)
    {
        var departmentPosition = new DepartmentPosition(departmentId, positionId);

        return Result.Success<DepartmentPosition, Errors>(departmentPosition);
    }
}

