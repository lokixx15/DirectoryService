using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class Position
{
    private const int MAX_DESCRIPTION_LENGTH = 1000;

    private readonly List<DepartmentPosition> _departments;

    private Position(
        Guid? id,
        PositonName name, 
        string description, 
        bool isActive,
        IEnumerable<DepartmentPosition> departments)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;    
        UpdatedAt = CreatedAt;
        _departments = departments.ToList();
    }
                    
    public Guid Id { get; private set; }
    public PositonName Name { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<DepartmentPosition> Departments => _departments;

    public static Result<Position> Create(
        Guid? id,
        PositonName name,
        string description,
        bool isActive,
        IEnumerable<DepartmentPosition> departments)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Position>("Description cannot be empty or whitespace.");
        if (description.Length > MAX_DESCRIPTION_LENGTH)
            return Result.Failure<Position>("Description cannot be longer than 1000 characters.");

        var position = new Position(id, name, description, isActive, departments);

        return Result.Success(position);
    }
}

