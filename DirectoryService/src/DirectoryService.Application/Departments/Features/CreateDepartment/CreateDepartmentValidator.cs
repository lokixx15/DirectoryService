using DirectoryService.Domain.Departments.VO;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(command => command.Request)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.Request.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(command => command.Request.Identifier)
            .MustBeValueObject(DepartmentIdentifier.Create);

        RuleFor(command => command.Request.LocationIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"))
            .Must(dI => dI.Distinct().Count() == dI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .When(command => command.Request != null);
    }
}