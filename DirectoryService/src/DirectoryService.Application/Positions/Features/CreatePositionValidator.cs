using DirectoryService.Application.Validation;
using DirectoryService.Domain;
using DirectoryService.Domain.Positions.VO;
using FluentValidation;
using SharedKernel;

namespace DirectoryService.Application.Positions.Features;

public class CreatePositionValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionValidator()
    {
        RuleFor(command => command.CreatePositionDto.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(command => command.CreatePositionDto.Description)
            .Must(d => d.Length <= Constants.MAX_POSITION_DESCRIPTION_LENGTH)
                .WithError(GeneralErrors.ValueLengthIsNotValid(
                    Constants.MAX_POSITION_DESCRIPTION_LENGTH,
                    "Description"));

        RuleFor(command => command.CreatePositionDto.DepartmentIds)
            .Must(dI => dI.Distinct().Count() == dI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("DepartmentsIds"));
    }
}
