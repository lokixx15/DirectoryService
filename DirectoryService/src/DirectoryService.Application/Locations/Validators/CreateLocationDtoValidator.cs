using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using DirectoryService.Application.Validation;

namespace DirectoryService.Application.Locations.Validators;

public class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(dto => dto.Address)
            .MustBeValueObject(LocationAddress.Create);

        RuleFor(dto => dto.Timezone)
           .MustBeValueObject(LocationTimezone.Create);
    }
}