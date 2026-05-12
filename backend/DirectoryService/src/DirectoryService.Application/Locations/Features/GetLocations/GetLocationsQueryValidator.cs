using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(query => query.Request.Search)
            .MaximumLength(1000)
                .WithError(GeneralErrors.ValueLengthIsNotValid(1000, "Search"));

        RuleFor(query => query.Request.Pagination.Page)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page must be greater than 0", "Page"))
            .LessThanOrEqualTo(10000)
                .WithError(GeneralErrors.ValueIsNotValid("Page cannot exceed 10000", "Page"));

        RuleFor(query => query.Request.Pagination.Size)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page size must be greater than 0", "Page size"))
            .LessThanOrEqualTo(150)
                .WithError(GeneralErrors.ValueIsNotValid("Page size cannot exceed 150", "Page size"));
    }
}