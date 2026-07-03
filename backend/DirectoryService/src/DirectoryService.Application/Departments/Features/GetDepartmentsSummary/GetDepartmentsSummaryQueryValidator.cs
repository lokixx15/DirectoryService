using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public class GetDepartmentsSummaryQueryValidator : AbstractValidator<GetDepartmentsSummaryQuery>
{
    public GetDepartmentsSummaryQueryValidator()
    {
        RuleFor(query => query.Request.Search)
            .MaximumLength(1000)
                .WithError(GeneralErrors.ValueLengthIsNotValid(1000, "Search"));

        RuleFor(query => query.Request.Page)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page must be greater than 0", "Page"))
            .LessThanOrEqualTo(10000)
                .WithError(GeneralErrors.ValueIsNotValid("Page cannot exceed 10000", "Page"));

        RuleFor(query => query.Request.pageSize)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page size must be greater than 0", "Page size"))
            .LessThanOrEqualTo(150)
                .WithError(GeneralErrors.ValueIsNotValid("Page size cannot exceed 150", "Page size"));
    }
}
