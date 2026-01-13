using CSharpFunctionalExtensions;
using SharedKernel;
using System.Text.RegularExpressions;
using DirectoryService.Domain.Departments.VO;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;

namespace DirectoryService.Domain.Departments;

public class Department
{
    //ef core
    private Department() { }

    private readonly List<Department> _childrenDepartments = [];
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    private Department(
        Guid? id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        Guid? parentId,
        DepartmentPath path,
        short depth,
        IEnumerable<DepartmentLocation> locations) 
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        _locations = locations.ToList();
    }

    public Guid Id { get; private set; }
    public DepartmentName Name { get; private set; } = null!;
    public DepartmentIdentifier Identifier { get; private set; } = null!;
    public Guid? ParentId { get; private set; }
    public DepartmentPath Path { get; private set; } = null!;
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<Department> ChildrenDepartments => _childrenDepartments;
    public IReadOnlyList<DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;

    public static Result<Department, Errors> CreateParent(
        Guid? id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        IEnumerable<DepartmentLocation> locations)
    {
        var errors = new List<Error>();

        var path = DepartmentPath.Create(identifier.Value);

        if (path.IsFailure)
            return path.Error;

        if (errors.Any())
            return Result.Failure<Department, Errors>(errors);

        var department = new Department(id, name, identifier, null, path.Value, 0, locations);

        return department;
    }

    public static Result<Department, Errors> CreateChild(
        Guid? id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        Department parent,
        IEnumerable<DepartmentLocation> locations)
    {
        var errors = new List<Error>();

        var path = DepartmentPath.Create(identifier.Value, parent.Path.Value);

        if (path.IsFailure)
            return path.Error;

        if (errors.Any())
            return Result.Failure<Department, Errors>(errors);

        var depth = parent.Depth + 1;

        var department = new Department(id, name, identifier, parent.Id, path.Value, (short)depth, locations);

        return department;
    }
}