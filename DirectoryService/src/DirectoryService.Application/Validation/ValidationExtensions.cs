using SharedKernel;
using FluentValidation.Results;

namespace DirectoryService.Application;

public static class ValidationExtensions
{
    public static Errors ToErrors(this ValidationResult validationResult) =>
        validationResult.Errors.Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName)).ToList();
}
