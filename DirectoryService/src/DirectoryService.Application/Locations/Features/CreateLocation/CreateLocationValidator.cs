using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations.VO;
using FluentValidation;
using SharedKernel;

namespace DirectoryService.Application.Locations.Features.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(command => command.Request)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.Request.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(command => command.Request.AddressRequest)
            .MustBeValueObject((addressDto) => LocationAddress.Create(
                    addressDto.Country,
                    addressDto.City,
                    addressDto.Street,
                    addressDto.Building,
                    addressDto.Region,
                    addressDto.District,
                    addressDto.Apartment));

        RuleFor(command => command.Request.Timezone)
           .MustBeValueObject(LocationTimezone.Create);
    }
}