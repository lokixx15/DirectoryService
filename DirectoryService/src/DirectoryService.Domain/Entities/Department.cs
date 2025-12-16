using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace DirectoryService.Domain.Entities;

public class Department
{
    //ef core
    private Department() { }

    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    private Department(
        Guid? id,
        DepartmentName name,
        string identifier,
        Guid? parentId,
        DepartmentPath path,
        short depth,
        bool isActive,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions) 
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        _locations = locations.ToList();
        _positions = positions.ToList();
    }

    public Guid Id { get; private set; }
    public DepartmentName Name { get; private set; } = null!;
    public string Identifier { get; private set; } = string.Empty;
    public Guid? ParentId { get; private set; }
    public DepartmentPath Path { get; private set; } = null!;
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;

    public static Result<Department> Create(
        Guid? id,
        DepartmentName name,
        string identifier,
        Guid? parentId,
        DepartmentPath path,
        short depth,
        bool isActive,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        string isLatinPattern = @"^[A-Za-z]+$";

        if (string.IsNullOrWhiteSpace(identifier))
            return Result.Failure<Department>("Identifier cannot be empty or whitespace.");

        if (identifier.Length > Constants.MAX_DEPARTMENT_IDENTIFIER_LENGTH || identifier.Length < Constants.MIN_NAME_LENGTH)
            return Result.Failure<Department>("Identifier can be from 3 to 150 characters long.");

        if (!Regex.IsMatch(identifier, isLatinPattern))
            return Result.Failure<Department>("Identifier must be in Latin.");

        var departnent = new Department(id, name, identifier, parentId, path, depth, isActive, locations, positions);

        return Result.Success(departnent);
    }
}

