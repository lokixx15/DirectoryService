using SharedKernel;

namespace DirectoryService.Domain.Exceptions;

public class FailureException : Exception
{
    public Error Error { get; } = null!;
    public FailureException(Error error) : base(error.GetMessage()) 
    {
        Error = error;
    }
}