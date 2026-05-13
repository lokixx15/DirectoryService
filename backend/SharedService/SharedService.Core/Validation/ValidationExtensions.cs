using FluentValidation.Results;
using SharedService.SharedKernel;

namespace SharedService.Core.Validation;

public static class ValidationExtensions
{
    public static Errors ToErrors(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors.Select(e =>
        {
            if (!e.ErrorMessage.Contains(Error.SEPARATOR))
            {
                return Error.Validation(
                    e.ErrorCode,
                    e.ErrorMessage,
                    e.PropertyName);
            }
            else
            {
                var errorMessage = e.ErrorMessage;
                var error = Error.Deserialize(errorMessage);
                return Error.Validation(error.Code, error.Message, error.InvalidField);
            }
        }).ToList();

        return errors;
    }
}