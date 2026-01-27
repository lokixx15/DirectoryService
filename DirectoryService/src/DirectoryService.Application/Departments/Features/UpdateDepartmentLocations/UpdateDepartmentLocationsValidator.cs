using DirectoryService.Application.Validation;
using FluentValidation;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
        RuleFor(command => command)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.Request.locationIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"))
            .Must(lI => lI.Distinct().Count() == lI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .When(command => command != null);
    }
}