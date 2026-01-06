using DirectoryService.Contracts.Locations;
using FluentValidation;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations.VO;

namespace DirectoryService.Application.Locations.Features;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(dto => dto.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(dto => dto.Address)
            .MustBeValueObject(LocationAddress.Create);

        RuleFor(dto => dto.Timezone)
           .MustBeValueObject(LocationTimezone.Create);
    }
}