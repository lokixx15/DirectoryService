using SharedKernel;

namespace DirectoryService.Domain.Exceptions;

public class ValidationException : Exception
{
    public Error Error { get; } = null!;
    public ValidationException(Error error) : base(error.GetMessage()) 
    {
        Error = error;
    }
}