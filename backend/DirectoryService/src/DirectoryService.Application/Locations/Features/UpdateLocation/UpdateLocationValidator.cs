using DirectoryService.Domain.Locations.VO;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.UpdateLocation;

public sealed class UpdateLocationValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationValidator()
    {
        RuleFor(command => command.Request)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.Request.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(command => command.Request.Address)
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