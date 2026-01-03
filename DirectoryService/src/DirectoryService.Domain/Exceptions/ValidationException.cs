using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryService.Domain.Exceptions;

public class ValidationException : Exception
{
    public Error Error { get; } = null!;
    public ValidationException(Error error) : base(error.GetMessage()) 
    {
        Error = error;
    }
}
