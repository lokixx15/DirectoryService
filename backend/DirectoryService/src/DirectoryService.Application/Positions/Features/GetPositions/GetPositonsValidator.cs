using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions.Features.GetPositions;

public class GetPositionsValidator : AbstractValidator<GetPositionsQuery>
{
    public GetPositionsValidator()
    {
        RuleFor(query => query.Request.PageSize)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page size must be greater than 0", "Page size"))
            .LessThanOrEqualTo(150)
                .WithError(GeneralErrors.ValueIsNotValid("Page size cannot exceed 150", "Page size"));
    }
}