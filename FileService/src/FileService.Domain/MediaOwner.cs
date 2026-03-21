using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain;

public sealed record MediaOwner
{
    // ef core
    private MediaOwner() { }

    private static readonly HashSet<string> AllowedContexts =
    [
        "lesson",
        "course",
        "user",
        "department"
    ];

    public string Context { get; } = string.Empty;

    public Guid EntityId { get; }

    private MediaOwner(
        string context,
        Guid entityId)
    {
        Context = context;
        EntityId = entityId;
    }

    public static Result<MediaOwner, Error> Create(string context, Guid entityId)
    {
        if (!string.IsNullOrWhiteSpace(context) || context.Length > 50)
            return GeneralErrors.ValueLengthIsNotValid(50, nameof(context));

        if (entityId == Guid.Empty)
            return Error.Validation("value.is.empty", "Entity id cannot be empty", null);

        string normalizedContext = context.Trim().ToLowerInvariant();
        if (!AllowedContexts.Contains(normalizedContext))
            return GeneralErrors.ValueIsNotValid(nameof(context));

        return new MediaOwner(normalizedContext, entityId);
    }

    public static Result<MediaOwner, Error> ForLesson(Guid entityId) => Create("lesson", entityId);

    public static Result<MediaOwner, Error> ForCourse(Guid entityId) => Create("course", entityId);

    public static Result<MediaOwner, Error> ForUser(Guid entityId) => Create("user", entityId);

    public static Result<MediaOwner, Error> ForDepartment(Guid entityId) => Create("department", entityId);
}