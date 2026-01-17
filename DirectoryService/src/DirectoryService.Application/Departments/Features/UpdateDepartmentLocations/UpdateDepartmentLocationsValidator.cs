using DirectoryService.Application.Validation;
using FluentValidation;
using SharedKernel;
using System.Text.Json;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsValidator()
    {
        RuleFor(command => command.updateDepartmentLocationsDto)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.updateDepartmentLocationsDto.LocationIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("LocationsIds"))
            .Must(lI => lI.Distinct().Count() == lI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .When(command => command.updateDepartmentLocationsDto != null);
    }
}