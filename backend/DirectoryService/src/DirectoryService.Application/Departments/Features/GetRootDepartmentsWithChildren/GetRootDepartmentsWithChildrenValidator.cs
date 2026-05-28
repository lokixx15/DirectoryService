using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public class GetRootDepartmentsWithChildrenValidator : AbstractValidator<GetRootDepartmentsWithChildrenQuery>
{
    public GetRootDepartmentsWithChildrenValidator()
    {
        RuleFor(query => query.Request.Page)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page must be greater than 0", "Page"))
            .LessThanOrEqualTo(10000)
                .WithError(GeneralErrors.ValueIsNotValid("Page cannot exceed 10000", "Page"));

        RuleFor(query => query.Request.Size)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Page size must be greater than 0", "Page size"))
            .LessThanOrEqualTo(150)
                .WithError(GeneralErrors.ValueIsNotValid("Page size cannot exceed 150", "Page size"));

        RuleFor(query => query.Request.Prefetch)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Number of children received must be greater than 0", "Page size"))
            .LessThanOrEqualTo(150)
                .WithError(GeneralErrors.ValueIsNotValid("Number of children received cannot exceed 150", "Page size"));
    }
}