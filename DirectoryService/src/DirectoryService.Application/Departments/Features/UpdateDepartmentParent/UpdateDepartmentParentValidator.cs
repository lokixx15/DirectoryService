using DirectoryService.Application.Validation;
using FluentValidation;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentParent;

public class UpdateDepartmentParentValidator : AbstractValidator<UpdateDepartmentParentCommand>
{
    public UpdateDepartmentParentValidator()
    {
        RuleFor(command => command.updateDepartmentParentDto)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command)
            .Must(c => c.departmentId != c.updateDepartmentParentDto.parentId)
                .WithError(Error.Validation(null, "The department being updated cannot be the parent department.", null));
    }
}