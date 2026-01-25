using DirectoryService.Application.Validation;
using FluentValidation;
using DirectoryService.Domain.Departments.VO;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(command => command.CreateDepartmentDto)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.CreateDepartmentDto.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(command => command.CreateDepartmentDto.Identifier)
            .MustBeValueObject(DepartmentIdentifier.Create);

        RuleFor(command => command.CreateDepartmentDto.LocationIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"))
            .Must(dI => dI.Distinct().Count() == dI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .When(command => command.CreateDepartmentDto != null);
    }
}