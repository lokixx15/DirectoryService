using DirectoryService.Domain;
using DirectoryService.Domain.Positions.VO;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions.Features.CreatePosition;

public class CreateCommandPositionValidator : AbstractValidator<CreatePositionCommand>
{
    public CreateCommandPositionValidator()
    {
        RuleFor(command => command.Request)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.Request.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(command => command.Request.Description)
            .Must(d => d.Length <= Constants.MAX_POSITION_DESCRIPTION_LENGTH)
                .WithError(GeneralErrors.ValueLengthIsNotValid(
                    Constants.MAX_POSITION_DESCRIPTION_LENGTH,
                    "Description"));

        RuleFor(command => command.Request.DepartmentIds)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("DepartmentsIds"))
            .NotEmpty()
                .WithError(GeneralErrors.CollectionIsNullOrEmpty("DepartmentsIds"))
            .Must(dI => dI.Distinct().Count() == dI.Length)
                .WithError(GeneralErrors.CollectionContainsDuplicates("LocationIds"))
            .When(command => command.Request != null);
    }
}