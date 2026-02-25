using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using SharedKernel;

namespace DirectoryService.Application.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Errors>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod.Invoke(value);

            if (result.IsSuccess)
                return;

            foreach (var e in result.Error)
            {
                context.AddFailure(new ValidationFailure(e.InvalidField, e.Message)
                { ErrorCode = e.Code });
            }
        }); 
    }

    public static IRuleBuilderOptions<T, TElement> WithError<T, TElement>(
        this IRuleBuilderOptions<T, TElement> ruleBuilderOptions,
        Error error)
    {
        return ruleBuilderOptions.WithMessage(error.Serialize());
    }
}