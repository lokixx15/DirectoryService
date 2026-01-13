using DirectoryService.Application.Validation;
using FluentValidation;
using DirectoryService.Domain.Departments.VO;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(command => command.CreateDepartmentDto.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(command => command.CreateDepartmentDto.Identifier)
            .MustBeValueObject(DepartmentIdentifier.Create);

        RuleFor(command => command.CreateDepartmentDto.LocationIds)
            .Must(dI => dI.Distinct().Count() == dI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"));
    }
}
