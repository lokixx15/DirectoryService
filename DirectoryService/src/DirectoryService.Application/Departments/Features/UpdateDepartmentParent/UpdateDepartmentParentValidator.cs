using DirectoryService.Application.Validation;
using FluentValidation;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentParent;

public class UpdateDepartmentParentCommandValidator : AbstractValidator<UpdateDepartmentParentCommand>
{
    public UpdateDepartmentParentCommandValidator()
    {
        RuleFor(command => command)
            .Must(c => c.departmentId != c?.parentId)
                .WithError(Error.Validation(null, "The department being updated cannot be the parent department.", null));
    }
}