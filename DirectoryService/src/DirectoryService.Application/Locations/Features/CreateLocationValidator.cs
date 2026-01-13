using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations.VO;
using FluentValidation;
using SharedKernel;

namespace DirectoryService.Application.Locations.Features;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(command => command.CreateLocationDto)
            .NotEmpty()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.CreateLocationDto.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(command => command.CreateLocationDto.Address)
            .MustBeValueObject((addressDto) => LocationAddress.Create(
                    addressDto.Country,
                    addressDto.City,
                    addressDto.Street,
                    addressDto.Building,
                    addressDto.Region,
                    addressDto.District,
                    addressDto.Apartment));

        RuleFor(command => command.CreateLocationDto.Timezone)
           .MustBeValueObject(LocationTimezone.Create);
    }
}
